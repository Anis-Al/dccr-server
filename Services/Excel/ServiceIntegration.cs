using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DCCR_SERVER.Context;
using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Models.Statiques;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ExcelDataReader;
using System.Text.Json;
using DCCR_SERVER.Models.ValidationFichiers;
using static DCCR_SERVER.Models.enums.Enums;
using DCCR_SERVER.DTOs;
using Microsoft.AspNetCore.Http; 

namespace DCCR_SERVER.Services.Excel
{
    public class ServiceIntegration
    {
        private readonly BddContext _contexte;
        private readonly ILogger<ServiceIntegration> _journal;
        private readonly IConfiguration _configuration;
        public ServiceIntegration(BddContext contexte, ILogger<ServiceIntegration> journal, IConfiguration configuration)
        {
            _contexte = contexte;
            _journal = journal;
            _configuration = configuration;
        }

        public async Task<ImportResultDto> TraiterEtMettreEnAttenteFichierAsync(IFormFile fichierExcel, string idIntegrateur)
        {
            var guidSession = Guid.NewGuid();
            var dossierStockage = _configuration["StorageSettings:sous_repertoire_fichiers_entree"] ?? Path.Combine(Path.GetTempPath(), "DCCR_Uploads"); 
            if (!Directory.Exists(dossierStockage))
            {
                try { Directory.CreateDirectory(dossierStockage); } catch (Exception) { throw; }
            }
            var nomFichier = Path.GetFileName(fichierExcel.FileName);
            var cheminStockage = Path.Combine(dossierStockage, $"{guidSession}_{nomFichier}");

            try { using (var flux = new FileStream(cheminStockage, FileMode.Create)) { await fichierExcel.CopyToAsync(flux); } }
            catch (Exception ex) { throw new IOException($"Failed save file: {ex.Message}", ex); }

            var fichier = new FichierExcel
            {
                nom_fichier_excel = nomFichier,
                chemin_fichier_excel = cheminStockage,
                id_integrateur_excel = idIntegrateur,
                date_heure_integration_excel = DateTime.Now,
                id_session_import = guidSession,
                statut_import = StatutImport.Telechargement
            };

            try { _contexte.fichiers_excel.Add(fichier); await _contexte.SaveChangesAsync(); }
            catch (Exception) { if (File.Exists(cheminStockage)) try { File.Delete(cheminStockage); } catch { } throw; }

            List<donnees_brutes> lignesMiseEnAttente = null;
            List<ErreurExcel> erreurs = new List<ErreurExcel>();

            try
            {
                using (var flux = new FileStream(cheminStockage, FileMode.Open, FileAccess.Read))
                {
                    (lignesMiseEnAttente, erreurs) = await TraiterEtMettreEnAttenteFichierInterneAsyncV2(fichier, flux);
                }
            }
            catch (Exception)
            {
                fichier.statut_import = StatutImport.EchecValidation;
                await _contexte.SaveChangesAsync();
            }

            if (erreurs.Any() && fichier.statut_import == StatutImport.Telechargement)
            {
                fichier.statut_import = StatutImport.EchecValidation;
                await _contexte.SaveChangesAsync();
            }
            else if (!erreurs.Any()) 
            {
                if (fichier.statut_import == StatutImport.Telechargement)
                {
                    fichier.statut_import = StatutImport.PretPourConfirmation;
                    await _contexte.SaveChangesAsync();
                }
            }

            var erreursDb = await GetErreursPourFichierAsync(fichier.id_fichier_excel);

            //resultat a afficher sur controller lmaooo
            var result = new ImportResultDto
            {
                contientErreurs = erreursDb.Any(),
                Erreurs = erreursDb,
                ApercuDonnees = (lignesMiseEnAttente != null && lignesMiseEnAttente.Any() && !erreursDb.Any())
                       ? BuildLoanPreview(lignesMiseEnAttente)
                       : new List<LoanPreviewDto>(),
                IdExcel=fichier.id_fichier_excel
            };
            return result;
        }


        private async Task<(List<donnees_brutes> lignes, List<ErreurExcel> erreurs)> TraiterEtMettreEnAttenteFichierInterneAsyncV2(FichierExcel fichier, Stream fluxExcel)
        {
            var erreurs = new List<ErreurExcel>();
            List<MappingColonnes> mappings;
            try { mappings = await _contexte.mapping_colonnes.AsNoTracking().ToListAsync(); }
            catch (Exception ex) { erreurs.Add(new ErreurExcel { message_erreur = "Error loading config: Mappings unavailable." }); return (new List<donnees_brutes>(), erreurs); }

            if (!mappings.Any()) { erreurs.Add(new ErreurExcel { message_erreur = "Config error: No mappings." }); return (new List<donnees_brutes>(), erreurs); }

            var lignes = new List<Dictionary<string, string>>();
            var header = new List<string>();
            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(fluxExcel))
                {
                    if (reader.Read()) {
                        for (int i = 0; i < reader.FieldCount; i++)
                            header.Add(reader.GetValue(i)?.ToString()?.Trim() ?? $"Column_{i + 1}");
                    }
                    else {
                        erreurs.Add(new ErreurExcel { message_erreur = "File empty/no header." });
                        return (new List<donnees_brutes>(), erreurs);
                    }
                    while (reader.Read())
                    {
                        var ligne = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        bool rowHasData = false;
                        for (int i = 0; i < header.Count; i++)
                        {
                            string cellValue = (i < reader.FieldCount) ? reader.GetValue(i)?.ToString() : null;
                            ligne[header[i]] = cellValue;
                            if (!string.IsNullOrWhiteSpace(cellValue))
                                rowHasData = true;
                        }
                        if (!rowHasData) break;
                        lignes.Add(ligne);
                    }
                }
            }
            catch (Exception ex) { erreurs.Add(new ErreurExcel { message_erreur = $"Error reading Excel: {ex.Message}" }); return (new List<donnees_brutes>(), erreurs); }

            var lignesMiseEnAttente = new List<donnees_brutes>(lignes.Count);
            int numeroLigneExcelSource = 2;
            var propertyCache = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var correspondance in mappings)
            {
                if (!string.IsNullOrEmpty(correspondance.colonne_bdd) && !propertyCache.ContainsKey(correspondance.colonne_bdd))
                {
                    var prop = typeof(donnees_brutes).GetProperty(correspondance.colonne_bdd);
                    if (prop != null) propertyCache[correspondance.colonne_bdd] = prop;
                }
            }
            foreach (var ligneDictionnaire in lignes)
            {
                var miseEnAttente = new donnees_brutes
                {
                    id_import_excel = fichier.id_fichier_excel,
                    id_session_import = fichier.id_session_import,
                    ligne_original = numeroLigneExcelSource,
                    est_valide = true,
                    messages_validation = "[]"
                };
                var messagesValidationLigne = new List<string>();
                bool ligneValideActuellement = true;
                foreach (var correspondance in mappings)
                {
                    var colonneExcel = correspondance.colonne_excel;
                    var proprieteMiseEnAttente = correspondance.colonne_bdd;
                    string valeurBrute = null;
                    if (!string.IsNullOrEmpty(colonneExcel))
                        ligneDictionnaire.TryGetValue(colonneExcel, out valeurBrute);
                    else continue;
                    object valeurASet = valeurBrute;
                    if (!string.IsNullOrEmpty(proprieteMiseEnAttente))
                    {
                        if (!propertyCache.TryGetValue(proprieteMiseEnAttente, out var propriete))
                        {
                            propriete = miseEnAttente.GetType().GetProperty(proprieteMiseEnAttente);
                            if (propriete != null) propertyCache[proprieteMiseEnAttente] = propriete;
                        }
                        if (propriete != null && propriete.CanWrite)
                        {
                            try { propriete.SetValue(miseEnAttente, valeurASet); }
                            catch { ligneValideActuellement = false; messagesValidationLigne.Add($"Erreur interne assignation '{proprieteMiseEnAttente}'."); }
                        }
                        else if (propriete == null)
                        {
                            continue;
                        }
                    }
                    else continue;
                }
                miseEnAttente.est_valide = ligneValideActuellement;
                miseEnAttente.messages_validation = messagesValidationLigne.Any() ? JsonSerializer.Serialize(messagesValidationLigne) : "[]";
                lignesMiseEnAttente.Add(miseEnAttente);
                numeroLigneExcelSource++;
            }
            try
            {
                _contexte.table_intermediaire_traitement.AddRange(lignesMiseEnAttente);
                await _contexte.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                erreurs.Add(new ErreurExcel { message_erreur = $"Database constraint violation saving data: {dbEx.InnerException?.Message ?? dbEx.Message}" });
                return (lignesMiseEnAttente, erreurs);
            }
            catch (Exception ex)
            {
                erreurs.Add(new ErreurExcel { message_erreur = $"Database error saving initial data: {ex.Message}" });
                return (lignesMiseEnAttente, erreurs);
            }
            List<ErreurExcel> erreursValidation = new List<ErreurExcel>();
            try
            {
                erreursValidation = await ValiderAvecReglesAsync(lignesMiseEnAttente);
                if (erreursValidation.Any())
                {
                    var erreursToStore = new List<DCCR_SERVER.Models.ValidationFichiers.ErreurExcel>(erreursValidation.Count);
                    foreach (var err in erreursValidation)
                    {
                        erreursToStore.Add(new DCCR_SERVER.Models.ValidationFichiers.ErreurExcel
                        {
                            id_excel = fichier.id_fichier_excel,
                            id_regle = err.id_regle,
                            ligne_excel = err.ligne_excel,
                            nom_colonne = err.nom_colonne,
                            message_erreur = err.message_erreur,
                            valeur_erronee = err.valeur_erronee,
                            id_staging_raw_data = err.id_staging_raw_data
                        });
                    }
                    _contexte.erreurs_fichiers_excel.AddRange(erreursToStore);
                    await _contexte.SaveChangesAsync();
                }
                bool changesMade = false;
                var lignesByOriginal = lignesMiseEnAttente.ToDictionary(l => l.ligne_original);
                foreach (var erreur in erreursValidation)
                {
                    if (!lignesByOriginal.TryGetValue(erreur.ligne_excel, out var ligneAffectee)) continue;
                    if (ligneAffectee.est_valide)
                    {
                        ligneAffectee.est_valide = false;
                        changesMade = true;
                    }
                    List<string> messages = new List<string>();
                    if (!string.IsNullOrEmpty(ligneAffectee.messages_validation) && ligneAffectee.messages_validation != "[]")
                    {
                        try { messages = JsonSerializer.Deserialize<List<string>>(ligneAffectee.messages_validation) ?? new List<string>(); }
                        catch { messages = new List<string> { ligneAffectee.messages_validation }; }
                    }
                    if (!messages.Contains(erreur.message_erreur))
                    {
                        messages.Add(erreur.message_erreur);
                        ligneAffectee.messages_validation = JsonSerializer.Serialize(messages);
                        changesMade = true;
                    }
                    if (changesMade && _contexte.Entry(ligneAffectee).State == EntityState.Unchanged)
                    {
                        _contexte.Entry(ligneAffectee).State = EntityState.Modified;
                    }
                }
                if (changesMade)
                {
                    await _contexte.SaveChangesAsync();
                }
            }
            catch (Exception) { }
            List<ErreurExcel> erreursCoherence = new List<ErreurExcel>();
            try
            {
                var props = typeof(donnees_brutes).GetProperties();
                var participantFields = props.Where(p => Attribute.IsDefined(p, typeof(DCCR_SERVER.Models.Principaux.ChampCoherenceParticipantAttribute))).Select(p => p.Name).ToList();
                var participantKeyProp = props.FirstOrDefault(p => p.Name == "participant_cle");
                if (participantKeyProp == null) { return (lignesMiseEnAttente, erreurs); }
                else if (participantFields.Any())
                {
                    var groupes = lignesMiseEnAttente.GroupBy(l => participantKeyProp.GetValue(l, null)?.ToString());
                    var champPropCache = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
                    foreach (var champName in participantFields)
                    {
                        if (!champPropCache.ContainsKey(champName))
                        {
                            var prop = props.FirstOrDefault(p => p.Name == champName);
                            if (prop != null) champPropCache[champName] = prop;
                        }
                    }
                    foreach (var groupe in groupes)
                    {
                        if (string.IsNullOrWhiteSpace(groupe.Key)) continue;
                        foreach (var champName in participantFields)
                        {
                            if (!champPropCache.TryGetValue(champName, out var champProp) || champProp == null) { continue; }
                            var valeurs = groupe.Select(l => champProp.GetValue(l, null)?.ToString()).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToList();
                            if (valeurs.Count > 1)
                            {
                                bool changesMadeCoherence = false;
                                foreach (var ligneDuGroupe in groupe)
                                {
                                    var erreurCoherence = new ErreurExcel { ligne_excel = ligneDuGroupe.ligne_original, nom_colonne = champName, message_erreur = $"Incohérence participant {groupe.Key} sur '{champName}'. Valeurs: {string.Join(", ", valeurs)}.", valeur_erronee = string.Join(", ", valeurs), id_regle = null };
                                    erreursCoherence.Add(erreurCoherence);
                                    if (ligneDuGroupe.est_valide) { ligneDuGroupe.est_valide = false; changesMadeCoherence = true; }
                                    List<string> messages = new List<string>();
                                    if (!string.IsNullOrEmpty(ligneDuGroupe.messages_validation) && ligneDuGroupe.messages_validation != "[]") { try { messages = JsonSerializer.Deserialize<List<string>>(ligneDuGroupe.messages_validation) ?? new List<string>(); } catch { messages = new List<string> { ligneDuGroupe.messages_validation }; } }
                                    if (!messages.Contains(erreurCoherence.message_erreur)) { messages.Add(erreurCoherence.message_erreur); ligneDuGroupe.messages_validation = JsonSerializer.Serialize(messages); changesMadeCoherence = true; }
                                    if (changesMadeCoherence && _contexte.Entry(ligneDuGroupe).State == EntityState.Unchanged)
                                    {
                                        _contexte.Entry(ligneDuGroupe).State = EntityState.Modified;
                                    }
                                }
                            }
                        }
                    }
                    if (erreursCoherence.Any())
                    {
                        if (_contexte.ChangeTracker.HasChanges())
                        {
                            await _contexte.SaveChangesAsync();
                        }
                        erreurs.AddRange(erreursCoherence);
                    }
                }
            }
            catch (Exception ex)
            {
                erreurs.Add(new ErreurExcel { message_erreur = $"Error coherence check: {ex.Message}" });
            }
            return (lignesMiseEnAttente, erreurs);
        }


        public async Task<List<ErreurExcel>> ValiderAvecReglesAsync(List<donnees_brutes> lignes)
        {
            var erreurs = new List<ErreurExcel>();
            List<RegleValidation> regles;
            try { regles = await _contexte.regles_validation.AsNoTracking().ToListAsync(); }
            catch (Exception ex) { return new List<ErreurExcel>(); } 
            if (regles == null || !regles.Any() || lignes == null || !lignes.Any()) return new List<ErreurExcel>();
            var propertyCache = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in regles)
            {
                if (!string.IsNullOrEmpty(r.nom_colonne) && !propertyCache.ContainsKey(r.nom_colonne))
                {
                    var prop = typeof(donnees_brutes).GetProperty(r.nom_colonne);
                    if (prop != null) propertyCache[r.nom_colonne] = prop;
                }
                if (!string.IsNullOrEmpty(r.colonne_dependante) && !propertyCache.ContainsKey(r.colonne_dependante))
                {
                    var prop = typeof(donnees_brutes).GetProperty(r.colonne_dependante);
                    if (prop != null) propertyCache[r.colonne_dependante] = prop;
                }
                if (!string.IsNullOrEmpty(r.colonne_cible) && !propertyCache.ContainsKey(r.colonne_cible))
                {
                    var prop = typeof(donnees_brutes).GetProperty(r.colonne_cible);
                    if (prop != null) propertyCache[r.colonne_cible] = prop;
                }
            }
            foreach (var ligne in lignes)
            {
                foreach (var regle in regles)
                {
                    var typeRegle = regle.type_regle?.Trim().ToUpperInvariant();
                    switch (typeRegle)
                    {
                        case "OBLIGATOIRE":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) continue;
                            object valeur = null;
                            try { valeur = prop.GetValue(ligne); } catch { }
                            string valeurStr = valeur?.ToString();
                            if (valeur == null || (valeur is string chaine1 && string.IsNullOrWhiteSpace(chaine1))) {
                                erreurs.Add(GenererErreur(ligne, regle, null));
                            }
                            break;
                        case "OBLIGATOIRE_SI":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurObligatoireSi = null;
                            try { valeurObligatoireSi = prop.GetValue(ligne); } catch { }
                            string valeurObligatoireSiStr = valeurObligatoireSi?.ToString();
                            if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                            {
                                if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) continue;
                                var valeurDep = propDep.GetValue(ligne);
                                var dependances = regle.valeur_dependante.Split(',');
                                if (valeurDep != null && dependances.Contains(valeurDep.ToString()))
                                {
                                    if (valeurObligatoireSi == null || (valeurObligatoireSi is string chaine2 && string.IsNullOrWhiteSpace(chaine2)))
                                    {
                                        erreurs.Add(GenererErreur(ligne, regle, valeurObligatoireSiStr));
                                    }
                                }
                            }
                            break;
                        case "TYPE_TEXTE":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurTypeTexte = null;
                            try { valeurTypeTexte = prop.GetValue(ligne); } catch { }
                            string valeurTypeTexteStr = valeurTypeTexte?.ToString();
                            if (valeurTypeTexte != null && !(valeurTypeTexte is string))
                            {
                                erreurs.Add(GenererErreur(ligne, regle, valeurTypeTexteStr));
                            }
                            break;
                        case "TYPE_DECIMAL":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurTypeDecimal = null;
                            try { valeurTypeDecimal = prop.GetValue(ligne); } catch { }
                            string valeurTypeDecimalStr = valeurTypeDecimal?.ToString();
                            if (!string.IsNullOrWhiteSpace(valeurTypeDecimalStr))
                            {
                                if (!decimal.TryParse(valeurTypeDecimalStr, out _))
                                {
                                    erreurs.Add(GenererErreur(ligne, regle, valeurTypeDecimalStr));
                                }
                            }
                            break;
                        case "TYPE_DATE":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurTypeDate = null;
                            try { valeurTypeDate = prop.GetValue(ligne); } catch { }
                            string valeurTypeDateStr = valeurTypeDate?.ToString();
                            if (!string.IsNullOrWhiteSpace(valeurTypeDateStr))
                            {
                                if (!DateOnly.TryParse(valeurTypeDateStr, out _))
                                {
                                    erreurs.Add(GenererErreur(ligne, regle, valeurTypeDateStr));
                                }
                            }
                            break;
                        case "TYPE_ENTIER":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurTypeEntier = null;
                            try { valeurTypeEntier = prop.GetValue(ligne); } catch { }
                            string valeurTypeEntierStr = valeurTypeEntier?.ToString();
                            if (!string.IsNullOrWhiteSpace(valeurTypeEntierStr))
                            {
                                if (!int.TryParse(valeurTypeEntierStr, out _))
                                {
                                    erreurs.Add(GenererErreur(ligne, regle, valeurTypeEntierStr));
                                }
                            }
                            break;
                        case "DOMAINE":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurDomaine = null;
                            try { valeurDomaine = prop.GetValue(ligne); } catch { }
                            string valeurDomaineStr = valeurDomaine?.ToString();
                            if (!string.IsNullOrWhiteSpace(valeurDomaineStr))
                            {
                                bool exists = false;
                                string tableName = regle.valeur_regle;
                                try
                                {
                                    switch (tableName)
                                    {
                                        case "monnaies":
                                            exists = _contexte.monnaies.Any(m => m.code == valeurDomaineStr);
                                            break;
                                        case "classes_retard":
                                            exists = _contexte.classes_retard.Any(x => x.code == valeurDomaineStr);
                                            break;
                                        case "types_garantie":
                                            exists = _contexte.types_garantie.Any(x => x.code == valeurDomaineStr);
                                            break;
                                        case "types_credit":
                                            exists = _contexte.types_credit.Any(x => x.code == valeurDomaineStr);
                                            break;
                                        case "situations_credit":
                                            exists = _contexte.situations_credit.Any(x => x.code == valeurDomaineStr);
                                            break;
                                        case "activites_credit":
                                            exists = _contexte.activites_credit.Any(x => x.code == valeurDomaineStr);
                                            break;
                                        case "wilayas":
                                            exists = _contexte.wilayas.Any(x => x.code == valeurDomaineStr);
                                            break;
                                        case "pays":
                                            exists = _contexte.pays.Any(x => x.code == valeurDomaineStr);
                                            break;
                                    }
                                }
                                catch { }
                                if (!exists)
                                {
                                    erreurs.Add(GenererErreur(ligne, regle, valeurDomaineStr));
                                }
                            }
                            break;
                        case "LONGUEUR":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurLongueur = null;
                            try { valeurLongueur = prop.GetValue(ligne); } catch { }
                            string valeurLongueurStr = valeurLongueur?.ToString();
                            if (valeurLongueurStr != null && valeurLongueurStr.Length != int.Parse(regle.valeur_regle))
                            {
                                erreurs.Add(GenererErreur(ligne, regle, valeurLongueurStr));
                            }
                            break;
                        case "VALEUR_PAR_DEFAUT_SI_VIDE":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurParDefaut = null;
                            try { valeurParDefaut = prop.GetValue(ligne); } catch { }
                            string valeurParDefautStr = valeurParDefaut?.ToString();
                            if (string.IsNullOrWhiteSpace(valeurParDefautStr))
                            {
                                var propDefaut = typeof(donnees_brutes).GetProperty(regle.nom_colonne);
                                if (propDefaut != null) propDefaut.SetValue(ligne, regle.valeur_regle);
                            }
                            break;
                        case "VALEUR_INTERDITE":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurInterdite = null;
                            try { valeurInterdite = prop.GetValue(ligne); } catch { }
                            string valeurInterditeStr = valeurInterdite?.ToString();
                            if (valeurInterdite != null && regle.valeur_regle.Split(',').Contains(valeurInterdite.ToString()))
                            {
                                erreurs.Add(GenererErreur(ligne, regle, valeurInterditeStr));
                            }
                            break;
                        case "VALEUR_INTERDITE_SI":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurInterditeSi = null;
                            try { valeurInterditeSi = prop.GetValue(ligne); } catch { }
                            string valeurInterditeSiStr = valeurInterditeSi?.ToString();
                            if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                            {
                                if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) continue;
                                var valeurDep = propDep.GetValue(ligne);
                                var dependances = regle.valeur_dependante.Split(',');
                                if (valeurDep != null && dependances.Contains(valeurDep.ToString()))
                                {
                                    if (valeurInterditeSi != null && regle.valeur_regle.Split(',').Contains(valeurInterditeSi.ToString()))
                                    {
                                        erreurs.Add(GenererErreur(ligne, regle, valeurInterditeSiStr));
                                    }
                                }
                            }
                            break;
                        case "VALEURS_INTERDITES_SI":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeursInterditesSi = null;
                            try { valeursInterditesSi = prop.GetValue(ligne); } catch { }
                            string valeursInterditesSiStr = valeursInterditesSi?.ToString();
                            if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                            {
                                if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) continue;
                                var valeurDep = propDep.GetValue(ligne);
                                var dependances = regle.valeur_dependante.Split(',');
                                if (valeurDep != null && dependances.Contains(valeurDep.ToString()))
                                {
                                    if (valeursInterditesSi != null && regle.valeur_regle.Split(',').Contains(valeursInterditesSi.ToString()))
                                    {
                                        erreurs.Add(GenererErreur(ligne, regle, valeursInterditesSiStr));
                                    }
                                }
                            }
                            break;
                        case "EGAL_A_SI":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurEgalASi = null;
                            try { valeurEgalASi = prop.GetValue(ligne); } catch { }
                            string valeurEgalASiStr = valeurEgalASi?.ToString();
                            if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                            {
                                if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) continue;
                                var valeurDep = propDep.GetValue(ligne);
                                var dependances = regle.valeur_dependante.Split(',');
                                if (valeurDep != null && dependances.Contains(valeurDep.ToString()))
                                {
                                    if (valeurEgalASi != null && valeurEgalASiStr != regle.valeur_regle)
                                    {
                                        erreurs.Add(GenererErreur(ligne, regle, valeurEgalASiStr));
                                    }
                                }
                            }
                            break;
                        case "SUP_A_SI":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurSupASi = null;
                            try { valeurSupASi = prop.GetValue(ligne); } catch { }
                            string valeurSupASiStr = valeurSupASi?.ToString();
                            if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                            {
                                if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) continue;
                                var valeurDep = propDep.GetValue(ligne);
                                var dependances = regle.valeur_dependante.Split(',');
                                if (valeurDep != null && dependances.Contains(valeurDep.ToString()))
                                {
                                    if (decimal.TryParse(valeurSupASiStr, out var valeurDecimale1) && decimal.TryParse(regle.valeur_regle, out var minimum1))
                                    {
                                        if (valeurDecimale1 <= minimum1)
                                        {
                                            erreurs.Add(GenererErreur(ligne, regle, valeurSupASiStr));
                                        }
                                    }
                                }
                            }
                            break;
                        case "SUP":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurSup = null;
                            try { valeurSup = prop.GetValue(ligne); } catch { }
                            string valeurSupStr = valeurSup?.ToString();
                            if (decimal.TryParse(valeurSupStr, out var valeurDecimale2) && decimal.TryParse(regle.valeur_regle, out var minimum2))
                            {
                                if (valeurDecimale2 <= minimum2)
                                {
                                    erreurs.Add(GenererErreur(ligne, regle, valeurSupStr));
                                }
                            }
                            break;
                        case "DOIT_ETRE_NULL_SI":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurDoitEtreNullSi = null;
                            try { valeurDoitEtreNullSi = prop.GetValue(ligne); } catch { }
                            string valeurDoitEtreNullSiStr = valeurDoitEtreNullSi?.ToString();
                            if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                            {
                                if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) continue;
                                var valeurDep = propDep.GetValue(ligne);
                                var dependances = regle.valeur_dependante.Split(',');
                                if (valeurDep != null && dependances.Contains(valeurDep.ToString()))
                                {
                                    if (valeurDoitEtreNullSi != null && !(valeurDoitEtreNullSi is string chaine3 && string.IsNullOrWhiteSpace(chaine3)))
                                    {
                                        erreurs.Add(GenererErreur(ligne, regle, valeurDoitEtreNullSiStr));
                                    }
                                }
                            }
                            break;
                        case "DOIT_ETRE_NULL_OU_ZERO_SI":
                            if (string.IsNullOrEmpty(regle.nom_colonne)) continue;
                            if (!propertyCache.TryGetValue(regle.nom_colonne, out prop)) continue;
                            object valeurDoitEtreNullOuZeroSi = null;
                            try { valeurDoitEtreNullOuZeroSi = prop.GetValue(ligne); } catch { }
                            string valeurDoitEtreNullOuZeroSiStr = valeurDoitEtreNullOuZeroSi?.ToString();
                            if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                            {
                                if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) continue;
                                var valeurDep = propDep.GetValue(ligne);
                                var dependances = regle.valeur_dependante.Split(',');
                                if (valeurDep != null && dependances.Contains(valeurDep.ToString()))
                                {
                                    if (!string.IsNullOrWhiteSpace(valeurDoitEtreNullOuZeroSiStr) && valeurDoitEtreNullOuZeroSiStr != "0")
                                    {
                                        erreurs.Add(GenererErreur(ligne, regle, valeurDoitEtreNullOuZeroSiStr));
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return erreurs;
        }

        private ErreurExcel GenererErreur(donnees_brutes ligne, RegleValidation regle, string valeurErronee)
        {
            string nomColonne = regle?.nom_colonne ?? "Inconnu"; 
            string messageRegle = regle?.message_erreur; 
            int? idRegle = regle?.id_regle;

            return new ErreurExcel { 
                ligne_excel = ligne.ligne_original, 
                nom_colonne = nomColonne, 
                message_erreur =  messageRegle, 
                id_regle = idRegle, 
                valeur_erronee = valeurErronee 
            };
        }

        public async Task<List<object>> GetErreursPourFichierAsync(int idExcel)
        {
            var resultatGroupe = await _contexte.erreurs_fichiers_excel
            .Where(e => e.id_excel == idExcel)
            .GroupBy(e => e.ligne_excel)
            .Select(groupe => new
            {
                ligne = groupe.Key,
                messages = groupe.Select(erreur => erreur.message_erreur).ToList()
            })
            .OrderBy(resultat => resultat.ligne)
            .ToListAsync();

            return resultatGroupe.Cast<object>().ToList();
        }

        public async Task<List<LoanPreviewDto>> GetPreviewPourFichierAsync(string idExcel)
        {
            if (!Guid.TryParse(idExcel, out var guidSession)) return new List<LoanPreviewDto>();
            var lignesValides = await _contexte.table_intermediaire_traitement
                .Where(x => x.id_session_import == guidSession && x.est_valide)
                .OrderBy(x => x.ligne_original)
                .ToListAsync();
            return BuildLoanPreview(lignesValides);
        }

        public List<LoanPreviewDto> BuildLoanPreview(List<donnees_brutes> lignesMiseEnAttente)
        {
            if (lignesMiseEnAttente == null || !lignesMiseEnAttente.Any()) return new List<LoanPreviewDto>();
            var lignesValides = lignesMiseEnAttente.Where(l => l.est_valide).ToList();
            if (!lignesValides.Any()) return new List<LoanPreviewDto>();
            try
            {
                return lignesValides.GroupBy(l => new { l.numero_contrat, l.date_declaration })
                    .Select(g =>
                    {
                        var first = g.First();
                        return new LoanPreviewDto
                        {
                            NumeroContrat = g.Key.numero_contrat,
                            DateDeclaration = g.Key.date_declaration,
                            SituationCredit = first.situation_credit,
                            DateOctroi = first.date_octroi,
                            DateRejet = first.date_rejet,
                            DateExpiration = first.date_expiration,
                            DateExecution = first.date_execution,
                            DureeInitiale = first.duree_initiale,
                            DureeRestante = first.duree_restante,
                            TypeCredit = first.type_credit,
                            ActiviteCredit = first.activite_credit,
                            Monnaie = first.monnaie,
                            CreditAccorde = first.credit_accorde,
                            IdPlafond = first.id_plafond,
                            Taux = first.taux,
                            Mensualite = first.mensualite,
                            CoutTotalCredit = first.cout_total_credit,
                            SoldeRestant = first.solde_restant,
                            ClasseRetard = first.classe_retard,
                            DateConstatation = first.date_constatation,
                            NombreEcheancesImpayes = first.nombre_echeances_impayes,
                            MontantInteretsCourus = first.montant_interets_courus,
                            MontantInteretsRetard = first.montant_interets_retard,
                            MontantCapitalRetard = first.montant_capital_retard,
                            Motif = first.motif,
                            CodeAgence = first.code_agence,
                            CodeWilaya = first.code_wilaya,
                            CodePays = first.code_pays,
                            Participants = g.GroupBy(l => l.participant_cle)
                                .Where(pg => !string.IsNullOrEmpty(pg.Key))
                                .Select(pg => pg.First())
                                .Select(p => new ParticipantPreviewDto
                                {
                                    ParticipantCle = p.participant_cle,
                                    ParticipantType = p.participant_type_cle,
                                    ParticipantNif = p.participant_nif,
                                    ParticipantCli = p.participant_cli,
                                    ParticipantRib = p.participant_rib,
                                    RoleNiveauResponsabilite = p.role_niveau_responsabilite
                                })
                                .ToList(),
                            Garanties = g.Where(l => l.type_garantie != null || l.montant_garantie != null)
                                .Select(l => new GarantiePreviewDto { TypeGarantie = l.type_garantie, MontantGarantie = l.montant_garantie })
                                .GroupBy(gar => new { gar.TypeGarantie, gar.MontantGarantie })
                                .Select(garGroup => garGroup.First()).ToList()
                        };
                    })
                    .OrderBy(l => l.DateDeclaration)
                    .ThenBy(l => l.NumeroContrat)
                    .ToList();
            }
            catch (Exception ex) { 
                return new List<LoanPreviewDto>(); 
            }
        }

    } 

  
} 