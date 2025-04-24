using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Reflection; 

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
                try { Directory.CreateDirectory(dossierStockage); } catch (Exception ex) { _journal.LogError(ex, "Failed create dir"); throw; }
            }
            var nomFichier = Path.GetFileName(fichierExcel.FileName);
            var cheminStockage = Path.Combine(dossierStockage, $"{guidSession}_{nomFichier}");

            try { using (var flux = new FileStream(cheminStockage, FileMode.Create)) { await fichierExcel.CopyToAsync(flux); } }
            catch (Exception ex) { _journal.LogError(ex, "Failed save file"); throw new IOException($"Failed save file: {ex.Message}", ex); }

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
            catch (Exception ex) { _journal.LogError(ex, "Failed save DB record"); if (File.Exists(cheminStockage)) try { File.Delete(cheminStockage); } catch { } throw; }

            List<donnees_brutes> lignesMiseEnAttente = null;
            List<ErreurExcel> erreurs = new List<ErreurExcel>();

            try
            {
                using (var flux = new FileStream(cheminStockage, FileMode.Open, FileAccess.Read))
                {
                    (lignesMiseEnAttente, erreurs) = await TraiterEtMettreEnAttenteFichierInterneAsyncV2(fichier, flux);
                }
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Error internal processing {FileId}", fichier.id_fichier_excel);
                erreurs.Add(new ErreurExcel { message_erreur = $"Internal processing error: {ex.Message}" });
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
            var result = new ImportResultDto
            {
                contientErreurs = erreursDb.Any(),
                Erreurs = erreursDb,
                ApercuDonnees = (lignesMiseEnAttente != null && lignesMiseEnAttente.Any())
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
            catch (Exception ex) { _journal.LogError(ex, "Failed load mappings."); erreurs.Add(new ErreurExcel { message_erreur = "Error loading config: Mappings unavailable." }); return (new List<donnees_brutes>(), erreurs); }

            if (!mappings.Any()) { erreurs.Add(new ErreurExcel { message_erreur = "Config error: No mappings." }); return (new List<donnees_brutes>(), erreurs); }

            var lignes = new List<Dictionary<string, string>>();
            var header = new List<string>();
            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(fluxExcel))
                {
                    if (reader.Read()) { 
                        for (int i = 0; i < reader.FieldCount; i++) 
                            header.Add(reader.GetValue(i)?
                                .ToString()?.Trim() ?? $"Column_{i + 1}"); 
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
                            {
                                rowHasData = true; 
                            }
                        }

                        if (!rowHasData)
                        {
                            _journal.LogInformation("Empty row encountered at or after Excel row {ExcelRow}. Stopping read.", lignes.Count + 2); 
                            break; 
                        }
                        lignes.Add(ligne);
                    }
                }
            }
            catch (Exception ex) { _journal.LogError(ex, "Error reading Excel {FileId}.", fichier.id_fichier_excel); erreurs.Add(new ErreurExcel { message_erreur = $"Error reading Excel: {ex.Message}" }); return (new List<donnees_brutes>(), erreurs); }

            var lignesMiseEnAttente = new List<donnees_brutes>();
            int numeroLigneExcelSource = 2;

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

                foreach (var mapping in mappings)
                {
                    var colonneExcel = mapping.colonne_excel;
                    var proprieteMiseEnAttente = mapping.colonne_bdd;
                    var typeAttendu = mapping.type_donnee;
                    string valeurBrute = null;

                    if (!string.IsNullOrEmpty(colonneExcel))
                    {
                        ligneDictionnaire.TryGetValue(colonneExcel, out valeurBrute);
                    }
                    else { continue; } 

                    object valeurASet = valeurBrute;
                    bool typeOk = true;

                    if (colonneExcel == "numero_contrat")
                    { 
                        _journal.LogDebug("Processing mapping for numero_contrat. BDD Column: '{BddCol}'. Value Read: '{ValRaw}'. Value To Set: '{ValASet}'",
                            proprieteMiseEnAttente, valeurBrute, valeurASet);
                    }

                    if (!string.IsNullOrEmpty(proprieteMiseEnAttente))
                    {
                        PropertyInfo propriete = null;
                        try { 
                            propriete = miseEnAttente.GetType().GetProperty(proprieteMiseEnAttente);
                            if (colonneExcel == "numero_contrat")
                            {
                                _journal.LogDebug("GetProperty result for '{PropName}': {IsNull}", proprieteMiseEnAttente, propriete == null ? "IS NULL" : "Found");
                            }
                        } 
                        catch (Exception ex)
                        {
                            if (colonneExcel == "numero_contrat")
                            {
                                _journal.LogError(ex, "Exception during GetProperty for {PropName}", proprieteMiseEnAttente);
                            }
                        }

                        if (propriete != null && propriete.CanWrite)
                        {
                            try
                            {
                                propriete.SetValue(miseEnAttente, valeurASet);
                            }
                            catch (Exception ex) 
                            {
                                _journal.LogWarning(ex, "SetValue failed for {Prop} on row {Row}", proprieteMiseEnAttente, numeroLigneExcelSource);
                                ligneValideActuellement = false; 
                                messagesValidationLigne.Add($"Erreur interne assignation '{proprieteMiseEnAttente}'.");
                            }
                        }
                        else if (propriete == null)
                        {
                            _journal.LogWarning("Mapped property '{Prop}' not found on donnees_brutes", proprieteMiseEnAttente);
                        }
                    }
                    else { continue; } 

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
                _journal.LogError(dbEx, "DbUpdateException during SAVE 1 for File ID {FileId}. InnerException: {InnerEx}", fichier.id_fichier_excel, dbEx.InnerException?.ToString());
                erreurs.Add(new ErreurExcel { message_erreur = $"Database constraint violation saving data: {dbEx.InnerException?.Message ?? dbEx.Message}" });
                return (lignesMiseEnAttente, erreurs); 
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Error saving initial staged data for file ID {FileId}", fichier.id_fichier_excel);
                erreurs.Add(new ErreurExcel { message_erreur = $"Database error saving initial data: {ex.Message}" });
                return (lignesMiseEnAttente, erreurs);
            }

            List<ErreurExcel> erreursValidation = new List<ErreurExcel>();
            try
            {
                erreursValidation = await ValiderAvecReglesAsync(lignesMiseEnAttente);

                if (erreursValidation.Any())
                {
                    var erreursToStore = new List<DCCR_SERVER.Models.ValidationFichiers.ErreurExcel>();
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
                foreach (var erreur in erreursValidation)
                {
                    var ligneAffectee = lignesMiseEnAttente.FirstOrDefault(l => l.ligne_original == erreur.ligne_excel);
                    if (ligneAffectee != null)
                    {
                        if (ligneAffectee.est_valide) 
                        {
                            ligneAffectee.est_valide = false;
                            changesMade = true;
                        }
                        List<string> messages = new List<string>();
                        if (!string.IsNullOrEmpty(ligneAffectee.messages_validation) && ligneAffectee.messages_validation != "[]") { try { messages = JsonSerializer.Deserialize<List<string>>(ligneAffectee.messages_validation) ?? new List<string>(); } catch { messages = new List<string> { ligneAffectee.messages_validation }; } }
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
                }
                if (changesMade)
                {
                    await _contexte.SaveChangesAsync(); 
                }
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Error validation/saving results {FileId}", fichier.id_fichier_excel);
                erreurs.Add(new ErreurExcel { message_erreur = $"Error validation processing: {ex.Message}" });
            }

            List<ErreurExcel> erreursCoherence = new List<ErreurExcel>();
            try
            {
                var participantFields = typeof(donnees_brutes).GetProperties().Where(p => Attribute.IsDefined(p, typeof(DCCR_SERVER.Models.Principaux.ChampCoherenceParticipantAttribute))).Select(p => p.Name).ToList();

                var participantKeyProp = typeof(donnees_brutes).GetProperty("participant_cle");
                if (participantKeyProp == null) { _journal.LogWarning("Prop 'participant_cle' missing."); }
                else if (participantFields.Any())
                {
                    var groupes = lignesMiseEnAttente.GroupBy(l => participantKeyProp.GetValue(l, null)?.ToString());
                    foreach (var groupe in groupes)
                    {
                        if (string.IsNullOrWhiteSpace(groupe.Key)) continue;
                        foreach (var champName in participantFields)
                        {
                            var champProp = typeof(donnees_brutes).GetProperty(champName);
                            if (champProp == null) { _journal.LogWarning("Coherence prop '{P}' missing.", champName); continue; }

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

                                    if (changesMadeCoherence && _contexte.Entry(ligneDuGroupe).State == EntityState.Unchanged) { _contexte.Entry(ligneDuGroupe).State = EntityState.Modified; }
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
                _journal.LogError(ex, "Probleme f coherence {FileId}", fichier.id_fichier_excel);
                erreurs.Add(new ErreurExcel { message_erreur = $"Error coherence check: {ex.Message}" });
            }

            return (lignesMiseEnAttente, erreurs);
        }


        private async Task<List<ErreurExcel>> ValiderAvecReglesAsync(List<donnees_brutes> lignes)
        {
            var erreurs = new List<ErreurExcel>();
            List<RegleValidation> regles;
            try { regles = await _contexte.regles_validation.AsNoTracking().ToListAsync(); }
            catch (Exception ex) { _journal.LogError(ex, "Failed load rules."); erreurs.Add(new ErreurExcel { message_erreur = "Error loading config: regles non trouvées." }); return erreurs; }

            if (!regles.Any() || lignes == null || !lignes.Any()) return erreurs;

            foreach (var ligne in lignes)
            {
                foreach (var regle in regles)
                {
                    string nomColonneRegle = regle.nom_colonne;
                    if (string.IsNullOrEmpty(nomColonneRegle)) { 
                        _journal.LogDebug("Skip Rule {ID}: no column.", regle.id_regle); 
                        continue; 
                    }

                    PropertyInfo prop = null;
                    try { prop = typeof(donnees_brutes).GetProperty(nomColonneRegle); } catch { }
                    if (prop == null) { _journal.LogWarning("Rule {ID} bad prop '{P}'.", regle.id_regle, nomColonneRegle); continue; }

                    object valeur = null;
                    try { valeur = prop.GetValue(ligne); } catch (Exception ex) { _journal.LogError(ex, "GetValue fail {P} row {R}", nomColonneRegle, ligne.ligne_original); continue; }
                    string valeurStr = valeur?.ToString();

                  
                    if (regle.categorie_regle == "REQUIRED") {
                         if (valeur == null || (valeur is string s && string.IsNullOrWhiteSpace(s))) { 
                            erreurs.Add(GenererErreur(ligne, regle, null)); 
                        } 
                    }
                    else if (regle.categorie_regle == "TYPE" && !string.IsNullOrEmpty(regle.valeur_regle)) {
                        bool typeErreur = false; string typeAttendu = regle.valeur_regle.ToLowerInvariant(); 
                        if (valeur != null) { 
                            try { 
                                switch (typeAttendu) { 
                                    case "int": case "integer": int.Parse(valeurStr); break; 
                                    case "decimal": decimal.Parse(valeurStr.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture); break; 
                                    case "date": case "dateonly": DateTime.Parse(valeurStr, System.Globalization.CultureInfo.InvariantCulture); break; 
                                    case "bool": bool.Parse(valeurStr); break; case "string": break; 
                                    default: break; 
                                } 
                            } 
                            catch { 
                                typeErreur = true; 
                            } 
                        } 
                        if (typeErreur) { 
                            erreurs.Add(GenererErreur(ligne, regle, valeurStr)); 
                        } 
                    }
                    else if (regle.categorie_regle == "LOOKUP" && !string.IsNullOrEmpty(regle.valeur_regle)) { 
                        if (valeur != null && !regle.valeur_regle.Split(',').Contains(valeurStr, StringComparer.OrdinalIgnoreCase)) { 
                            erreurs.Add(GenererErreur(ligne, regle, valeurStr)); 
                        } 
                    }
                    else if (regle.categorie_regle == "DEPENDENCY" && !string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                    {
                        PropertyInfo propDep = null; try { propDep = typeof(donnees_brutes).GetProperty(regle.colonne_dependante); } catch { }
                        if (propDep != null)
                        {
                            var valeurDep = propDep.GetValue(ligne);
                            if (valeurDep != null && valeurDep.ToString().Equals(regle.valeur_dependante, StringComparison.OrdinalIgnoreCase))
                            {
                                bool conditionViolated = (valeur == null) || (regle.valeur_regle != null && !valeurStr.Equals(regle.valeur_regle, StringComparison.OrdinalIgnoreCase));
                                if (conditionViolated) { erreurs.Add(GenererErreur(ligne, regle, valeurStr)); }
                            }
                        }
                        else { _journal.LogWarning("Rule {ID} bad dep col '{C}'.", regle.id_regle, regle.colonne_dependante); }
                    }
                    else if (regle.categorie_regle == "CONSISTENCY" && !string.IsNullOrEmpty(regle.colonne_cible) && !string.IsNullOrEmpty(regle.valeur_cible_attendue))
                    {
                        PropertyInfo propCible = null; try { propCible = typeof(donnees_brutes).GetProperty(regle.colonne_cible); } catch { }
                        if (propCible != null)
                        {
                            var valeurCible = propCible.GetValue(ligne);
                            if (valeur != null && valeurCible != null && !valeurStr.Equals(regle.valeur_regle, StringComparison.OrdinalIgnoreCase) 
                                && !valeurCible.ToString().Equals(regle.valeur_cible_attendue, StringComparison.OrdinalIgnoreCase))
                            {
                                erreurs.Add(GenererErreur(ligne, regle, valeurStr));
                            }
                        }
                        else { _journal.LogWarning("Rule {ID} bad target col '{C}'.", regle.id_regle, regle.colonne_cible); }
                    }
                    else if (regle.type_regle == "NULL_IF_NOT" && regle.categorie_regle == "DEPENDENCY" && !string.IsNullOrEmpty(regle.colonne_dependante))
                    {
                        PropertyInfo propDep = null; try { propDep = typeof(donnees_brutes).GetProperty(regle.colonne_dependante); } catch { }
                        if (propDep != null)
                        {
                            var valeurDep = propDep.GetValue(ligne);
                            if (valeurDep == null || !valeurDep.ToString().Equals(regle.valeur_dependante, StringComparison.OrdinalIgnoreCase))
                            { if (valeur != null && !(valeur is string s && string.IsNullOrWhiteSpace(s))) { erreurs.Add(GenererErreur(ligne, regle, valeurStr)); } }
                        }
                        else { _journal.LogWarning("Rule {ID} bad dep col '{C}'.", regle.id_regle, regle.colonne_dependante); }
                    }
                    else if (regle.categorie_regle == "GUARANTOR")
                    {
                        var propNiveau = typeof(donnees_brutes).GetProperty("role_niveau_responsabilite");
                        var propTypeGarantie = typeof(donnees_brutes).GetProperty("garantie_type_garantie");
                        var propMontantGarantie = typeof(donnees_brutes).GetProperty("garantie_montant_garantie");
                        if (propNiveau != null && propTypeGarantie != null && propMontantGarantie != null)
                        {
                            var niveauVal = propNiveau.GetValue(ligne)?.ToString();
                            if (niveauVal != "5") 
                            {
                                var valTypeGarantie = propTypeGarantie.GetValue(ligne); 
                                if (valTypeGarantie != null && !(valTypeGarantie is string s && string.IsNullOrWhiteSpace(s))) { 
                                    erreurs.Add(GenererErreur(ligne, regle, valTypeGarantie.ToString())); 
                                }
                                var valMontantGarantie = propMontantGarantie.GetValue(ligne); 
                                if (valMontantGarantie != null && !(valMontantGarantie is decimal d && d == 0) && !(valMontantGarantie is string ms && string.IsNullOrWhiteSpace(ms))) { 
                                    erreurs.Add(GenererErreur(ligne, regle, valMontantGarantie.ToString())); }
                            }
                        }
                        else { _journal.LogWarning("Guarantor rule props missing."); }
                    }
                    else if (regle.type_regle == "DOMAINES" && regle.categorie_regle == "SIMPLE" && !string.IsNullOrEmpty(regle.valeur_regle))
                    { 
                        if (!string.IsNullOrWhiteSpace(valeurStr))
                        {
                            bool exists = false;
                            string tableName = regle.valeur_regle;
                            try
                            {
                                switch (tableName)
                                {
                                    case "monnaies":
                                        exists = _contexte.monnaies.Any(m => m.code == valeurStr);
                                        break;
                                    case "classes_retard":
                                        exists = _contexte.classes_retard.Any(x => x.code == valeurStr);
                                        break;
                                    case "types_garantie":
                                        exists = _contexte.types_garantie.Any(x => x.code == valeurStr);
                                        break;
                                    case "types_credit":
                                        exists = _contexte.types_credit.Any(x => x.code == valeurStr);
                                        break;
                                    case "situations_credit":
                                        exists = _contexte.situations_credit.Any(x => x.code == valeurStr);
                                        break;
                                    case "activites_credit":
                                        exists = _contexte.activites_credit.Any(x => x.code == valeurStr);
                                        break;
                                    case "wilayas":
                                        exists = _contexte.wilayas.Any(x => x.code == valeurStr);
                                        break;
                                    case "pays":
                                        exists = _contexte.pays.Any(x => x.code == valeurStr);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception dbEx)
                            {
                                _journal.LogError(dbEx, "erreur au domaines");
                            }
                            if (!exists)
                            {
                                erreurs.Add(GenererErreur(ligne, regle, valeurStr));
                            }
                        }
                    }

                } 
            } 

            var consistencyRules = regles.Where(r => r.categorie_regle == "CONSISTENCY").ToList();
            if (consistencyRules.Any()) { 
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
                    .Select(g => new LoanPreviewDto { 
                        NumeroContrat = g.Key.numero_contrat, DateDeclaration = g.Key.date_declaration, Participants = g.GroupBy(l => l.participant_cle)
                        .Where(pg => !string.IsNullOrEmpty(pg.Key))
                        .Select(pg => pg.First())
                        .Select(p => new ParticipantPreviewDto { 
                            ParticipantCle = p.participant_cle, 
                            ParticipantType = p.participant_type_cle, 
                            ParticipantNif = p.participant_nif, 
                            ParticipantCli = p.participant_cli, 
                            ParticipantRib = p.participant_rib 
                        })
                        .ToList(),
                        Garanties = g.Where(l => l.garantie_type_garantie != null || l.garantie_montant_garantie != null)
                        .Select(l => new GarantiePreviewDto { TypeGarantie = l.garantie_type_garantie, MontantGarantie = l.garantie_montant_garantie })
                        .GroupBy(gar => new { gar.TypeGarantie, gar.MontantGarantie })
                        .Select(garGroup => garGroup.First()).ToList() })
                        .OrderBy(l => l.DateDeclaration)
                        .ThenBy(l => l.NumeroContrat).ToList();
            }
            catch (Exception ex) { _journal.LogError(ex, "Error building preview."); return new List<LoanPreviewDto>(); }
        }

    } 

  
} 