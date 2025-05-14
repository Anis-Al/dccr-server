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
using Microsoft.AspNetCore.Http;
using DCCR_SERVER.DTOs.Excel;

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
            var dossierStockage = _configuration["StorageSettings:sous_repertoire_fichiers_entree"] ?? Path.Combine(Path.GetTempPath(), "fichiers .xslx du dccr"); 
            if (!Directory.Exists(dossierStockage))
            {
                try { Directory.CreateDirectory(dossierStockage); } catch (Exception) { throw; }
            }
            var nomFichier = Path.GetFileName(fichierExcel.FileName);
            var cheminStockage = Path.Combine(dossierStockage, nomFichier);

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
                    (lignesMiseEnAttente, erreurs) = await LireParStreamsEtDetecterErreurs(fichier, flux);
                }
            }
            catch (Exception)
            {
                fichier.statut_import = StatutImport.EchecValidation;
                File.Delete(cheminStockage);
                await _contexte.SaveChangesAsync();
            }
            if (erreurs.Any() && fichier.statut_import == StatutImport.Telechargement)
            {
                fichier.statut_import = StatutImport.EchecValidation;
                File.Delete(cheminStockage);
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
                       ? construireApercuUi(lignesMiseEnAttente)
                       : new List<ApercuCredit>(),
                IdExcel=fichier.id_fichier_excel,
                NomFichierExcel=fichier.nom_fichier_excel
            };
            return result;
        }

        private async Task<(List<donnees_brutes> lignes, List<ErreurExcel> erreurs)> LireParStreamsEtDetecterErreurs(FichierExcel fichier, Stream fluxExcel)
        {
            var erreurs = new List<ErreurExcel>();
            List<MappingColonnes> mappings;
            try { mappings = await _contexte.mapping_colonnes.AsNoTracking().ToListAsync(); }
            catch (Exception ex) { return (new List<donnees_brutes>(), erreurs); }

            if (!mappings.Any()) { return (new List<donnees_brutes>(), erreurs); }

            var lignes = new List<Dictionary<string, string>>();
            var header = new List<string>();
            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(fluxExcel)) 
                {
                    if (reader.Read()) {
                        for (int i = 0; i < reader.FieldCount; i++)
                            header.Add(reader.GetValue(i)?.ToString()?.Trim());
                    }
                    else {
                        erreurs.Add(new ErreurExcel { message_erreur = "Fichier vide!" });
                        return (new List<donnees_brutes>(), erreurs);
                    }
                    while (reader.Read())
                    {
                        var ligne = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        bool donneeExiste = false;
                        for (int i = 0; i < header.Count; i++)
                        {
                            string cellValue = (i < reader.FieldCount) ? reader.GetValue(i)?.ToString() : null;
                            ligne[header[i]] = cellValue;
                            if (!string.IsNullOrWhiteSpace(cellValue))
                                donneeExiste = true;
                        }
                        if (!donneeExiste) break;
                        lignes.Add(ligne);
                    }
                }
            }
            catch (Exception ex) {return (new List<donnees_brutes>(), erreurs); }

            var lignesMiseEnAttente = new List<donnees_brutes>(lignes.Count);
            int numeroLigneExcelSource = 2;
            var donneeCache = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var correspondance in mappings)
            {
                if (!string.IsNullOrEmpty(correspondance.colonne_bdd) && !donneeCache.ContainsKey(correspondance.colonne_bdd))
                {
                    var prop = typeof(donnees_brutes).GetProperty(correspondance.colonne_bdd);
                    if (prop != null) donneeCache[correspondance.colonne_bdd] = prop;
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
                };
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
                        if (!donneeCache.TryGetValue(proprieteMiseEnAttente, out var propriete))
                        {
                            propriete = miseEnAttente.GetType().GetProperty(proprieteMiseEnAttente);
                            if (propriete != null) donneeCache[proprieteMiseEnAttente] = propriete;
                        }
                        if (propriete != null && propriete.CanWrite)
                        {
                            try { propriete.SetValue(miseEnAttente, valeurASet); }
                            catch { ligneValideActuellement = false;  }
                        }
                        else if (propriete == null)
                        {
                            continue;
                        }
                    }
                    else continue;
                }
                miseEnAttente.est_valide = ligneValideActuellement;
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
                erreurs.Add(new ErreurExcel { message_erreur = $" {dbEx.InnerException?.Message ?? dbEx.Message}" });
                return (lignesMiseEnAttente, erreurs);
            }
            catch (Exception ex)
            {
                erreurs.Add(new ErreurExcel { message_erreur = $" {ex.Message}" });
                return (lignesMiseEnAttente, erreurs);
            }

            // where on verifie les erreurs de validation (selon notice technique et xsd crem)
            List<ErreurExcel> erreursValidation = new List<ErreurExcel>();
            try
            {
                erreursValidation = await ValiderAvecReglesAsync(lignesMiseEnAttente);
                if (erreursValidation.Any())
                {
                    var erreurs_a_stocker = new List<ErreurExcel>(erreursValidation.Count);
                    foreach (var err in erreursValidation)
                    {
                        erreurs_a_stocker.Add(new ErreurExcel
                        {
                            id_excel = fichier.id_fichier_excel,
                            id_regle = err.id_regle,
                            ligne_excel = err.ligne_excel,
                            message_erreur = err.message_erreur,
                        });
                    }
                    _contexte.erreurs_fichiers_excel.AddRange(erreurs_a_stocker);
                    await _contexte.SaveChangesAsync();
                }
                bool changementFait = false;
                var lignesByOriginal = lignesMiseEnAttente.ToDictionary(l => l.ligne_original);
                foreach (var erreur in erreursValidation)
                {
                    if (!lignesByOriginal.TryGetValue(erreur.ligne_excel, out var ligneAffectee)) continue;
                    if (ligneAffectee.est_valide)
                    {
                        ligneAffectee.est_valide = false;
                        changementFait = true;
                    }
                    
                    if (changementFait && _contexte.Entry(ligneAffectee).State == EntityState.Unchanged)
                    {
                        _contexte.Entry(ligneAffectee).State = EntityState.Modified;
                    }
                }
                if (changementFait)
                {
                    await _contexte.SaveChangesAsync();
                }
            }
            catch (Exception) { }
           
            return (lignesMiseEnAttente, erreurs);
        }

        public async Task<List<ErreurExcel>> ValiderAvecReglesAsync(List<donnees_brutes> lignes)
        {
            var erreurs = new List<ErreurExcel>();
            List<RegleValidation> regles;
            try { regles = await _contexte.regles_validation.AsNoTracking().ToListAsync(); }
            catch { return new List<ErreurExcel>(); }
            if (regles == null || !regles.Any() || lignes == null || !lignes.Any()) 
            return new List<ErreurExcel>();
            var donnee_cache = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in regles)
            {
                if (!string.IsNullOrEmpty(r.nom_colonne) && !donnee_cache.ContainsKey(r.nom_colonne))
                {
                    var prop = typeof(donnees_brutes).GetProperty(r.nom_colonne);
                    if (prop != null) donnee_cache[r.nom_colonne] = prop;
                }
                if (!string.IsNullOrEmpty(r.colonne_dependante) && !donnee_cache.ContainsKey(r.colonne_dependante))
                {
                    var prop = typeof(donnees_brutes).GetProperty(r.colonne_dependante);
                    if (prop != null) donnee_cache[r.colonne_dependante] = prop;
                }
                if (!string.IsNullOrEmpty(r.colonne_cible) && !donnee_cache.ContainsKey(r.colonne_cible))
                {
                    var prop = typeof(donnees_brutes).GetProperty(r.colonne_cible);
                    if (prop != null) donnee_cache[r.colonne_cible] = prop;
                }
            }
            var hashDureesCredit = _contexte.durees_credit.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashNiveauxResponsabilite = _contexte.niveaux_responsabilite.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashMonnaies = _contexte.monnaies.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashClassesRetard = _contexte.classes_retard.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashTypesGarantie = _contexte.types_garantie.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashTypesCredit = _contexte.types_credit.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashSituationsCredit = _contexte.situations_credit.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashActivitesCredit = _contexte.activites_credit.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashWilayas = _contexte.wilayas.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashPays = _contexte.pays.AsNoTracking().Select(x => x.code).ToHashSet();
            var hashAgences = _contexte.agences.AsNoTracking().Select(x => x.code).ToHashSet();
            foreach (var ligne in lignes)
            {
                foreach (var regle in regles)
                {
                    var typeRegle = regle.type_regle?.Trim().ToUpperInvariant();
                    switch (typeRegle)
                    {
                        case "OBLIGATOIRE":
                            TraiterObligatoire(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "OBLIGATOIRE_SI":
                            TraiterObligatoireSi(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "TYPE":
                            TraiterType(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "DOMAINE":
                            TraiterDomaine(erreurs, ligne, regle, donnee_cache, hashDureesCredit, hashNiveauxResponsabilite, hashMonnaies, hashClassesRetard, hashTypesGarantie, hashTypesCredit, hashSituationsCredit, hashActivitesCredit, hashAgences, hashWilayas, hashPays);
                            break;
                        case "FORMAT":
                            TraiterFormat(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "LONGUEUR":
                            TraiterLongueur(erreurs, ligne, regle, donnee_cache);
                            break;
                        
                        case "VALEUR_INTERDITE":
                            TraiterValeurInterdite(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "VALEURS_INTERDITES_SI_PAS":
                            TraiterValeursInterditesSiPas(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "VALEURS_INTERDITES_SI":
                            TraiterValeursInterditesSi(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "EGAL_A_SI":
                            TraiterEgalASi(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "SUP_A_SI":
                            TraiterSupASi(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "SUP":
                            TraiterSup(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "DOIT_ETRE_NULL_SI":
                            TraiterDoitEtreNullSi(erreurs, ligne, regle, donnee_cache);
                            break;
                        case "DOIT_ETRE_NULL_OU_ZERO_SI":
                            TraiterDoitEtreNullOuZeroSi(erreurs, ligne, regle, donnee_cache);
                            break;
                        default:
                            break;
                    }
                }
            }
            var erreurs_coherence_participants = verifierCoherenceParticipantCredit(lignes);
            if (erreurs_coherence_participants.Any())
            erreurs.AddRange(erreurs_coherence_participants);

            var erreurs_coherence_credits = verifierCoherenceCredit(lignes);
            if (erreurs_coherence_credits.Any())
            erreurs.AddRange(erreurs_coherence_credits);

            return erreurs;
        }

        private ErreurExcel GenererErreur(donnees_brutes ligne, RegleValidation regle, string valeurErronee)
        {
            string messageRegle = regle.message_erreur; 
            int? idRegle = regle?.id_regle;

            return new ErreurExcel { 
                ligne_excel = ligne.ligne_original, 
                message_erreur =  messageRegle, 
                id_regle = idRegle, 
            };
        }

        public async Task<List<object>> GetErreursPourFichierAsync(int idExcel)
        {
            return await _contexte.erreurs_fichiers_excel
            .Where(e => e.id_excel == idExcel)
            .AsNoTracking() 
            .GroupBy(e => e.ligne_excel)
            .Select(groupe => new
            {
                ligne = groupe.Key,
                messages = groupe.Select(erreur => erreur.message_erreur).ToList()
            })
            .OrderBy(resultat => resultat.ligne)
            .ToListAsync<object>();
        }

        public async Task<List<ApercuCredit>> commencerConstruireApercuUi(string idExcel)
        {
            int.TryParse(idExcel,out int id);
            var lignesValides = await _contexte.table_intermediaire_traitement
                .AsNoTracking()
                .Where(x => x.id_import_excel == id && x.est_valide)
                .OrderBy(x => x.ligne_original)
                .ToListAsync();
            return construireApercuUi(lignesValides);
        }

        public List<ApercuCredit> construireApercuUi(List<donnees_brutes> lignesMiseEnAttente)
        {
            if (lignesMiseEnAttente?.Any() != true) return new List<ApercuCredit>();
            try
            {
                var groupedByContract = lignesMiseEnAttente
                    .GroupBy(l => new { l.numero_contrat, l.date_declaration })
                    .ToList();

                var result = new List<ApercuCredit>(groupedByContract.Count);

                foreach (var g in groupedByContract)
                {
                    var col = g.First();

                    var participantsFiltered = g.Where(l => !string.IsNullOrEmpty(l.participant_cle)).ToList();
                    var participantsGrouped = participantsFiltered
                        .GroupBy(l => l.participant_cle)
                        .ToList();
                 
                    var apercuCredit = new ApercuCredit
                    {
                        NumeroContrat = col.numero_contrat,
                        DateDeclaration = col.date_declaration,
                        TypeCredit = col.type_credit,
                        SituationCredit = col.situation_credit,
                        ActiviteCredit = col.activite_credit,
                        ClasseRetard = col.classe_retard,
                        Monnaie = col.monnaie,
                        Taux = col.taux,
                        CreditAccorde = col.credit_accorde,
                        Mensualite = col.mensualite,
                        CoutTotalCredit = col.cout_total_credit,
                        SoldeRestant = col.solde_restant,
                        MontantCapitalRetard = col.montant_capital_retard,
                        MontantInteretsCourus = col.montant_interets_courus,
                        MontantInteretsRetard = col.montant_interets_retard,
                        DateOctroi = col.date_octroi,
                        DateRejet = col.date_rejet,
                        DateConstatation=col.date_constatation,
                        DateExecution=col.date_execution,
                        DateExpiration=col.date_expiration,
                        DureeInitiale = col.duree_initiale,
                        DureeRestante = col.duree_restante,
                        CodeAgence = col.code_agence,
                        CodeWilaya = col.code_wilaya,
                        CodePays = col.code_pays,

                        Participants = participantsGrouped.Select(pg => new ApercuParticipant
                        {
                            ParticipantCle = pg.Key,
                            ParticipantType = pg.First().participant_type_cle,
                            RoleNiveauResponsabilite = pg.First().role_niveau_responsabilite,
                            ParticipantCli=pg.First().participant_cli,
                            ParticipantNif= pg.First().participant_nif,
                            ParticipantRib = pg.First().participant_rib,
                        }).ToList(),

                        Garanties = g.Where(l => l.type_garantie != null || l.montant_garantie != null)
                                .Select(l => new ApercuGarantie { TypeGarantie = l.type_garantie, MontantGarantie = l.montant_garantie })
                                .GroupBy(gar => new { gar.TypeGarantie, gar.MontantGarantie })
                                .Select(garGroup => garGroup.First()).ToList()
                    };

                    result.Add(apercuCredit);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new List<ApercuCredit>();
            }
        }

        public async Task MigrerDonneesStagingVersProdAsync(int idExcel)
        {
            var pasValide = await _contexte.table_intermediaire_traitement
                .AnyAsync(x => x.id_import_excel == idExcel && !x.est_valide);

            if (pasValide)
            {
                throw new InvalidOperationException("");
            }

            using var transaction = await _contexte.Database.BeginTransactionAsync();
            try
            {
                var mappings = await _contexte.mapping_colonnes.AsNoTracking().ToListAsync();
                var mappingsParTable = mappings
                    .Where(m => !string.IsNullOrEmpty(m.table_prod))
                    .GroupBy(m => m.table_prod)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var donnees_en_attentes_de_migration = await _contexte.table_intermediaire_traitement
                    .Where(x => x.id_import_excel == idExcel)
                    .ToListAsync();

                if (!donnees_en_attentes_de_migration.Any())
                {
                    await transaction.CommitAsync();
                    return;
                }

                var uniqueAgenceCodes = donnees_en_attentes_de_migration
                    .Select(l => l.code_agence?.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .ToList();

                var existingAgenceCodes = new HashSet<string>();
                if (uniqueAgenceCodes.Any())
                {
                    existingAgenceCodes = (await _contexte.agences
                       .Where(a => uniqueAgenceCodes.Contains(a.code))
                       .Select(a => a.code)
                       .ToListAsync())
                       .ToHashSet();
                }

                var lieuxDict = new Dictionary<(string, string, string), Lieu>();
                var uniqueLieuKeys = new HashSet<(string codePays, string codeWilaya, string codeAgence)>();

                foreach (var ligne in donnees_en_attentes_de_migration)
                {
                    var codePays = (ligne.code_pays ?? "").Trim();
                    var codeWilaya = (ligne.code_wilaya ?? "").Trim();
                    var codeAgenceRaw = (ligne.code_agence ?? "").Trim();
                    var codeAgenceToUse = !string.IsNullOrEmpty(codeAgenceRaw) && existingAgenceCodes.Contains(codeAgenceRaw)
                                          ? codeAgenceRaw
                                          : null;
                    uniqueLieuKeys.Add((codePays, codeWilaya, codeAgenceToUse));
                }

                var existingLieux = await _contexte.lieux
                    .Where(l => uniqueLieuKeys.Select(k => k.codePays).Contains(l.code_pays) &&
                                 uniqueLieuKeys.Select(k => k.codeWilaya).Contains(l.code_wilaya))
                    .ToListAsync();

                var existingLieuxDict = existingLieux
                   .ToDictionary(l => (l.code_pays ?? "", l.code_wilaya ?? "", l.code_agence), l => l);

                int lieuxAddedCount = 0;
                foreach (var key in uniqueLieuKeys)
                {
                    if (existingLieuxDict.TryGetValue(key, out var existingLieu))
                    {
                        lieuxDict[key] = existingLieu;
                    }
                    else
                    {
                        var newLieu = new Lieu
                        {
                            code_pays = key.codePays,
                            code_wilaya = key.codeWilaya,
                            code_agence = key.codeAgence
                        };
                        _contexte.lieux.Add(newLieu);
                        lieuxDict[key] = newLieu;
                        lieuxAddedCount++;

                    }
                }

                var intervenantsDict = new Dictionary<string, Intervenant>();
                var uniqueIntervenantCles = donnees_en_attentes_de_migration
                    .Select(l => l.participant_cle?.Trim())
                    .Where(cle => !string.IsNullOrEmpty(cle))
                    .Distinct()
                    .ToList();

                if (uniqueIntervenantCles.Any() && mappingsParTable.TryGetValue("intervenants", out var intervenantMappings))
                {
                    var existingIntervenants = await _contexte.intervenants
                        .Where(i => uniqueIntervenantCles.Contains(i.cle))
                        .ToListAsync();

                    intervenantsDict = existingIntervenants.ToDictionary(i => i.cle, i => i);

                    foreach (var ligne in donnees_en_attentes_de_migration)
                    {
                        var intervenantCle = ligne.participant_cle?.Trim();
                        if (string.IsNullOrEmpty(intervenantCle) || intervenantsDict.ContainsKey(intervenantCle))
                        {
                            continue;
                        }

                        var intervenant = new Intervenant();
                        mapperColonnesAvecTypesEnProd(intervenantMappings, ligne, intervenant);
                        var mappedCle = intervenant.cle?.Trim();
                        if (!string.IsNullOrEmpty(mappedCle) && !intervenantsDict.ContainsKey(mappedCle))
                        {
                            if (mappedCle == intervenantCle)
                            {
                                _contexte.intervenants.Add(intervenant);
                                intervenantsDict[mappedCle] = intervenant;
                            }
                            else
                            {
                            }
                        }
                    }
                }

                var creditsDict = new Dictionary<(string, DateOnly?, int), Crédit>();

                if (mappingsParTable.TryGetValue("credits", out var creditMappings))
                {
                    foreach (var ligne in donnees_en_attentes_de_migration)
                    {
                        var credit = new Crédit();
                        mapperColonnesAvecTypesEnProd(creditMappings, ligne, credit);

                        var codePays = (ligne.code_pays ?? "").Trim();
                        var codeWilaya = (ligne.code_wilaya ?? "").Trim();
                        var codeAgenceRaw = (ligne.code_agence ?? "").Trim();
                        var codeAgenceToUse = !string.IsNullOrEmpty(codeAgenceRaw) && existingAgenceCodes.Contains(codeAgenceRaw)
                                              ? codeAgenceRaw
                                              : null;
                        var cleLieu = (codePays, codeWilaya, codeAgenceToUse);

                        if (lieuxDict.TryGetValue(cleLieu, out var lieu))
                        {
                            credit.lieu = lieu;
                        }
                        else
                        {
                        }

                        credit.id_excel = idExcel;
                        _contexte.Set<Crédit>().Add(credit);

                        DateOnly? dateDeclarationParse = null;
                        if (!string.IsNullOrEmpty(ligne.date_declaration) && DateOnly.TryParse(ligne.date_declaration, out DateOnly parsedDate))
                        {
                            dateDeclarationParse = parsedDate;
                        }

                        var numeroContratTrimmed = ligne.numero_contrat?.Trim();
                        var cleCredit = (numeroContratTrimmed, dateDeclarationParse, idExcel);

                        if (!string.IsNullOrEmpty(cleCredit.Item1) && cleCredit.Item2.HasValue)
                        {
                            creditsDict[cleCredit] = credit;
                        }
                        else
                        {

                        }
                    }
                }

                if (mappingsParTable.TryGetValue("intervenants_credits", out var icMappings))
                {
                    foreach (var ligne in donnees_en_attentes_de_migration)
                    {
                        var intervenantCredit = new IntervenantCrédit();
                        mapperColonnesAvecTypesEnProd(icMappings, ligne, intervenantCredit);

                        var intervenantCle = ligne.participant_cle?.Trim();
                        string numContratCoupe = ligne.numero_contrat?.Trim();
                        DateOnly? dateDeclarationParse = null;
                        if (!string.IsNullOrEmpty(ligne.date_declaration) && DateOnly.TryParse(ligne.date_declaration, out DateOnly parsedDate))
                        {
                            dateDeclarationParse = parsedDate;
                        }

                        var creditKey = (numContratCoupe, dateDeclarationParse, idExcel);

                        Intervenant intervenant = null;
                        Crédit credit = null;

                        bool intevenantTrouve = !string.IsNullOrEmpty(intervenantCle) && intervenantsDict.TryGetValue(intervenantCle, out intervenant);
                        bool cleCreditValide = !string.IsNullOrEmpty(creditKey.Item1) && creditKey.Item2.HasValue;
                        bool creditTrouve = cleCreditValide && creditsDict.TryGetValue(creditKey, out credit);

                        if (intevenantTrouve)
                        {
                            intervenantCredit.intervenant = intervenant;
                        }
                        else if (!string.IsNullOrEmpty(intervenantCle))
                        {
                        }

                        if (creditTrouve)
                        {
                            intervenantCredit.credit = credit;
                        }
                        else if (cleCreditValide)
                        {
                        }

                        if (intervenantCredit.intervenant != null && intervenantCredit.credit != null)
                        {
                            _contexte.Set<IntervenantCrédit>().Add(intervenantCredit);
                        }
                        else
                        {
                        }
                    }
                }

                if (mappingsParTable.TryGetValue("garanties", out var garantieMappings))
                {
                    foreach (var ligne in donnees_en_attentes_de_migration)
                    {
                        var garantie = new Garantie();
                        mapperColonnesAvecTypesEnProd(garantieMappings, ligne, garantie);

                        var intervenantCle = ligne.participant_cle?.Trim();
                        string numeroContratTrimmed = ligne.numero_contrat?.Trim();
                        DateOnly? dateDeclarationParsed = null;
                        if (!string.IsNullOrEmpty(ligne.date_declaration) && DateOnly.TryParse(ligne.date_declaration, out DateOnly parsedDate))
                        {
                            dateDeclarationParsed = parsedDate;
                        }
                        var cleCredit = (numeroContratTrimmed, dateDeclarationParsed, idExcel);

                        Intervenant guarantor = null;
                        Crédit credit = null;

                        bool guarantTrouve = !string.IsNullOrEmpty(intervenantCle) && intervenantsDict.TryGetValue(intervenantCle, out guarantor);
                        bool cleCreditValide = !string.IsNullOrEmpty(cleCredit.Item1) && cleCredit.Item2.HasValue;
                        bool creditTrouve = cleCreditValide && creditsDict.TryGetValue(cleCredit, out credit);


                        if (guarantTrouve)
                        {
                            garantie.guarant = guarantor;
                        }
                        else if (!string.IsNullOrEmpty(intervenantCle))
                        {

                        }

                        if (creditTrouve)
                        {
                            garantie.credit = credit;
                        }
                        else if (cleCreditValide)
                        {

                        }

                        garantie.id_excel = idExcel;

                        if (garantie.credit != null)
                        {
                            _contexte.Set<Garantie>().Add(garantie);
                        }
                        else
                        {
                        }
                    }
                }

                await _contexte.SaveChangesAsync();

                _contexte.table_intermediaire_traitement.RemoveRange(donnees_en_attentes_de_migration);
                await _contexte.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Erreur lors de la migration des données .", ex);
            }
        }


        #region methodes_controle


        public List<ErreurExcel> verifierCoherenceParticipantCredit(List<donnees_brutes> lignes)
                {
                    var erreursCoherence = new List<ErreurExcel>();
                    var props = typeof(donnees_brutes).GetProperties();
                    var participantFields = props
                        .Where(p => Attribute.IsDefined(p, typeof(DCCR_SERVER.Models.Principaux.ChampCoherenceParticipantAttribute)))
                        .ToList();
                    var participantKeyProp = props.FirstOrDefault(p => p.Name == "participant_cle");
                    if (participantKeyProp == null || !participantFields.Any()) return erreursCoherence;

                    var groupes = lignes.GroupBy(l => participantKeyProp.GetValue(l, null)?.ToString());
                    foreach (var groupe in groupes)
                    {
                        if (string.IsNullOrWhiteSpace(groupe.Key)) continue;
                        foreach (var champProp in participantFields)
                        {
                            var valeurs = groupe.Select(l => champProp.GetValue(l, null)?.ToString()).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToList();
                            if (valeurs.Count > 1)
                            {
                                foreach (var ligneDuGroupe in groupe)
                                {
                                    erreursCoherence.Add(new ErreurExcel
                                    {
                                        ligne_excel = ligneDuGroupe.ligne_original,
                                        message_erreur = $"Incohérence participant {groupe.Key} sur '{champProp.Name}'. Valeurs: {string.Join(", ", valeurs)}.",
                                        id_regle = null
                                    });
                                    ligneDuGroupe.est_valide = false;
                                }
                            }
                        }
                    }
                    return erreursCoherence;
                }


                public List<ErreurExcel> verifierCoherenceCredit(List<donnees_brutes> lignes)
                {
                    var erreursCoherence = new List<ErreurExcel>();
                    var props = typeof(donnees_brutes).GetProperties();
                    var creditFields = props
                        .Where(p => Attribute.IsDefined(p, typeof(ChampCoherenceAttribute)))
                        .ToList();
                    var numeroContratProp = props.FirstOrDefault(p => p.Name == "numero_contrat");
                    if (numeroContratProp == null || !creditFields.Any()) return erreursCoherence;

                    var groupes = lignes.GroupBy(l =>
                        numeroContratProp.GetValue(l, null)?.ToString());
                    foreach (var groupe in groupes)
                    {
                        if (string.IsNullOrWhiteSpace(groupe.Key)) continue;
                        foreach (var champProp in creditFields)
                        {
                            var valeurs = groupe.Select(l => champProp.GetValue(l, null)?.ToString()).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToList();
                            if (valeurs.Count > 1)
                            {
                                foreach (var ligneDuGroupe in groupe)
                                {
                                    erreursCoherence.Add(new ErreurExcel
                                    {
                                        ligne_excel = ligneDuGroupe.ligne_original,
                                        message_erreur = $"Incohérence crédit {groupe.Key} sur '{champProp.Name}'. Valeurs: {string.Join(", ", valeurs)}.",
                                        id_regle = null
                                    });
                                    ligneDuGroupe.est_valide = false;
                                }
                            }
                        }
                    }
                    return erreursCoherence;
                }

                void TraiterObligatoire(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeur = null;
                    try { valeur = prop.GetValue(ligne); } catch { }
                    if (valeur == null || (valeur is string chaine1 && string.IsNullOrWhiteSpace(chaine1))) {
                        erreurs.Add(GenererErreur(ligne, regle, null));
                    }
                }

                void TraiterObligatoireSi(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeurObligatoireSi = null;
                    try { valeurObligatoireSi = prop.GetValue(ligne); } catch { }
                    bool allDepsOk = true;
                    if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                    {
                        var colonnes = regle.colonne_dependante.Split('|');
                        var valeurs = regle.valeur_dependante.Split('|');
                        if (colonnes.Length != valeurs.Length)
                        {
                            allDepsOk = false;
                        }
                        else
                        {
                            for (int i = 0; i < colonnes.Length; i++)
                            {
                                var col = colonnes[i].Trim();
                                var vals = valeurs[i].Split(',').Select(v => v.Trim()).ToList();
                                if (!propertyCache.TryGetValue(col, out var propDep)) { allDepsOk = false; break; }
                                var valeurDep = propDep.GetValue(ligne);
                                if (valeurDep == null || !vals.Contains(valeurDep.ToString())) { allDepsOk = false; break; }
                            }
                        }
                    }
                    if (allDepsOk)
                    {
                        if (valeurObligatoireSi == null || (valeurObligatoireSi is string chaine2 && string.IsNullOrWhiteSpace(chaine2)))
                        {
                            erreurs.Add(GenererErreur(ligne, regle, valeurObligatoireSi?.ToString()));
                        }
                    }
                }

                void TraiterType(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne) || string.IsNullOrEmpty(regle.valeur_regle)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeur = null;
                    try { valeur = prop.GetValue(ligne); } catch { }
                    var valeurStr = valeur?.ToString();
                    if (!string.IsNullOrWhiteSpace(valeurStr))
                    {
                        var type = regle.valeur_regle.ToLower();
                        if (!string.IsNullOrWhiteSpace(valeurStr))
                        {
                            switch (type)
                            {
                                case "entier":
                                    if (!int.TryParse(valeurStr, out _))
                                        erreurs.Add(GenererErreur(ligne, regle, valeurStr));
                                    break;
                                case "decimal":
                                    if (!decimal.TryParse(valeurStr, out _))
                                        erreurs.Add(GenererErreur(ligne, regle, valeurStr));
                                    break;
                                case "dateonly":
                                    if (!DateOnly.TryParse(valeurStr, out _))
                                        erreurs.Add(GenererErreur(ligne, regle, valeurStr));
                                    break;
                                
                                default:
                                    break;
                            }
                        }
                    }
                }

                void TraiterDomaine(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache,
                    HashSet<string> hashDureesCredit, HashSet<string> hashNiveauxResponsabilite, HashSet<string> hashMonnaies, HashSet<string> hashClassesRetard,
                    HashSet<string> hashTypesGarantie, HashSet<string> hashTypesCredit, HashSet<string> hashSituationsCredit, HashSet<string> hashActivitesCredit, HashSet<string> hashAgences,
                    HashSet<string> hashWilayas, HashSet<string> hashPays)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeurDomaine = null;
                    try { valeurDomaine = prop.GetValue(ligne); } catch { }
                    var valeurDomaineStr = valeurDomaine?.ToString();
                    if (!string.IsNullOrWhiteSpace(valeurDomaineStr))
                    {
                        bool exists = false;
                        string tableName = regle.valeur_regle;
                        switch (tableName)
                        {
                            case "durees_credit": exists = hashDureesCredit.Contains(valeurDomaineStr); break;
                            case "niveaux_responsabilite": exists = hashNiveauxResponsabilite.Contains(valeurDomaineStr); break;
                            case "monnaies": exists = hashMonnaies.Contains(valeurDomaineStr); break;
                            case "classes_retard": exists = hashClassesRetard.Contains(valeurDomaineStr); break;
                            case "types_garantie": exists = hashTypesGarantie.Contains(valeurDomaineStr); break;
                            case "types_credit": exists = hashTypesCredit.Contains(valeurDomaineStr); break;
                            case "situations_credit": exists = hashSituationsCredit.Contains(valeurDomaineStr); break;
                            case "activites_credit": exists = hashActivitesCredit.Contains(valeurDomaineStr); break;
                            case "wilayas": exists = hashWilayas.Contains(valeurDomaineStr); break;
                            case "pays": exists = hashPays.Contains(valeurDomaineStr); break;
                            case "agences": exists = hashAgences.Contains(valeurDomaineStr);break;
                        }
                        if (!exists)
                        {
                            erreurs.Add(GenererErreur(ligne, regle, valeurDomaineStr));
                        }
                    }
                }
                void TraiterFormat(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne) || string.IsNullOrEmpty(regle.valeur_regle)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeurFormat = null;
                    try { valeurFormat = prop.GetValue(ligne); } catch { }
                    var valeurFormatStr = valeurFormat?.ToString();
                    if (!string.IsNullOrEmpty(valeurFormatStr))
                    {
                        try
                        {
                            if (!System.Text.RegularExpressions.Regex.IsMatch(valeurFormatStr, regle.valeur_regle))
                            {
                                erreurs.Add(GenererErreur(ligne, regle, valeurFormatStr));
                            }
                        }
                        catch { erreurs.Add(GenererErreur(ligne, regle, valeurFormatStr)); }
                    }
                }

                void TraiterLongueur(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeurLongueur = null;
                    try { valeurLongueur = prop.GetValue(ligne); } catch { }
                    var valeurLongueurStr = valeurLongueur?.ToString();
                    if (valeurLongueurStr != null && valeurLongueurStr.Length != int.Parse(regle.valeur_regle))
                    {
                        erreurs.Add(GenererErreur(ligne, regle, valeurLongueurStr));
                    }
                }

                // void TraiterValeurParDefautSiVide(donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                // {
                //     if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                //     if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                //     object valeurParDefaut = null;
                //     try { valeurParDefaut = prop.GetValue(ligne); } catch { }
                //     var valeurParDefautStr = valeurParDefaut?.ToString();
                //     if (string.IsNullOrWhiteSpace(valeurParDefautStr))
                //     {
                //         var propDefaut = typeof(donnees_brutes).GetProperty(regle.nom_colonne);
                //         if (propDefaut != null) propDefaut.SetValue(ligne, regle.valeur_regle);
                //     }
                // }

                void TraiterValeurInterdite(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeurInterdite = null;
                    try { valeurInterdite = prop.GetValue(ligne); } catch { }
                    if (valeurInterdite != null && regle.valeur_regle.Split(',').Contains(valeurInterdite.ToString()))
                    {
                        erreurs.Add(GenererErreur(ligne, regle, valeurInterdite?.ToString()));
                    }
                }

                //void TraiterValeurInterditeSi(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                //{
                //    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                //    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                //    object valeurInterditeSi = null;
                //    try { valeurInterditeSi = prop.GetValue(ligne); } catch { }
                //    if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                //    {
                //        if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) return;
                //        var valeurDep = propDep.GetValue(ligne);
                //        var dependances = regle.valeur_dependante.Split(',');
                //        if (valeurDep != null && dependances.Contains(valeurDep.ToString()))
                //        {
                //            if (valeurInterditeSi != null && regle.valeur_regle.Split(',').Contains(valeurInterditeSi.ToString()))
                //            {
                //                erreurs.Add(GenererErreur(ligne, regle, valeurInterditeSi?.ToString()));
                //            }
                //        }
                //    }
                //}

                void TraiterValeursInterditesSi(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeursInterditesSi = null;
                    try { valeursInterditesSi = prop.GetValue(ligne); } catch { }
                    if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                    {
                        if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) return;
                        var valeurDep = propDep.GetValue(ligne);
                        var dependances = regle.valeur_dependante.Split(',');
                        if (valeurDep != null && dependances.Contains(valeurDep.ToString()))
                        {
                            if (valeursInterditesSi != null && regle.valeur_regle.Split(',').Contains(valeursInterditesSi.ToString()))
                            {
                                erreurs.Add(GenererErreur(ligne, regle, valeursInterditesSi?.ToString()));
                            }
                        }
                    }
                }

                void TraiterValeursInterditesSiPas(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeursInterditesSi = null;
                    try { valeursInterditesSi = prop.GetValue(ligne); } catch { }
                    if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                    {
                        if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) return;
                        var valeurDep = propDep.GetValue(ligne);
                        var dependances = regle.valeur_dependante.Split(',');
                        if (valeurDep != null && !dependances.Contains(valeurDep.ToString()))
                        {
                            if (valeursInterditesSi != null && regle.valeur_regle.Split(',').Contains(valeursInterditesSi.ToString()))
                            {
                                erreurs.Add(GenererErreur(ligne, regle, valeursInterditesSi?.ToString()));
                            }
                        }
                    }
                }
                void TraiterEgalASi(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne) ||
                        !propertyCache.TryGetValue(regle.nom_colonne, out var prop))
                        return;

                    object valeurEgalASi = null;
                    try { valeurEgalASi = prop.GetValue(ligne); }
                    catch { }

                    string valeurEgalASiStr = valeurEgalASi?.ToString();

                    // Extended: Multi-condition dependency check
                    bool allDepsOk = true;
                    if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                    {
                        var colonnes = regle.colonne_dependante.Split('|');
                        var valeurs = regle.valeur_dependante.Split('|');
                        for (int i = 0; i < colonnes.Length; i++)
                        {
                            var col = colonnes[i].Trim();
                            var val = valeurs.Length > i ? valeurs[i].Trim() : "";
                            if (!propertyCache.TryGetValue(col, out var propDep)) { allDepsOk = false; break; }
                            var valeurDep = propDep.GetValue(ligne)?.ToString();

                            // Handle != and = logic, and multiple values
                            if (val.StartsWith("!="))
                            {
                                var notVals = val.Substring(2).Split(',').Select(v => v.Trim()).ToList();
                                if (valeurDep == null || notVals.Contains(valeurDep)) { allDepsOk = false; break; }
                            }
                            else if (val.Contains(','))
                            {
                                var inVals = val.Split(',').Select(v => v.Trim()).ToList();
                                if (valeurDep == null || !inVals.Contains(valeurDep)) { allDepsOk = false; break; }
                            }
                            else
                            {
                                if (valeurDep == null || valeurDep != val) { allDepsOk = false; break; }
                            }
                        }
                    }
                    if (allDepsOk)
                    {
                        if (valeurEgalASi == null || valeurEgalASiStr != regle.valeur_regle)
                            erreurs.Add(GenererErreur(ligne, regle, valeurEgalASiStr));
                    }
                }

                void TraiterSupASi(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeurSupASi = null;
                    try { valeurSupASi = prop.GetValue(ligne); } catch { }
                    var valeurSupASiStr = valeurSupASi?.ToString();

                    bool allDepsOk = true;
                    if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                    {
                        var colonnes = regle.colonne_dependante.Split('|');
                        var valeurs = regle.valeur_dependante.Split('|');
                        for (int i = 0; i < colonnes.Length; i++)
                        {
                            var col = colonnes[i].Trim();
                            var val = valeurs.Length > i ? valeurs[i].Trim() : "";
                            if (!propertyCache.TryGetValue(col, out var propDep)) { allDepsOk = false; break; }
                            var valeurDep = propDep.GetValue(ligne)?.ToString();

                            // Handle != and = logic, and multiple values
                            if (val.StartsWith("!="))
                            {
                                var notVals = val.Substring(2).Split(',').Select(v => v.Trim()).ToList();
                                if (valeurDep == null || notVals.Contains(valeurDep)) { allDepsOk = false; break; }
                            }
                            else if (val.Contains(','))
                            {
                                var inVals = val.Split(',').Select(v => v.Trim()).ToList();
                                if (valeurDep == null || !inVals.Contains(valeurDep)) { allDepsOk = false; break; }
                            }
                            else
                            {
                                if (valeurDep == null || valeurDep != val) { allDepsOk = false; break; }
                            }
                        }
                    }
                    if (allDepsOk)
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

                void TraiterSup(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeurSup = null;
                    try { valeurSup = prop.GetValue(ligne); } catch { }
                    var valeurSupStr = valeurSup?.ToString();
                    if (decimal.TryParse(valeurSupStr, out var valeurDecimale2) && decimal.TryParse(regle.valeur_regle, out var minimum2))
                    {
                        if (valeurDecimale2 <= minimum2)
                        {
                            erreurs.Add(GenererErreur(ligne, regle, valeurSupStr));
                        }
                    }
                }

                void TraiterDoitEtreNullSi(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeurDoitEtreNullSi = null;
                    try { valeurDoitEtreNullSi = prop.GetValue(ligne); } catch { }
                    var valeurDoitEtreNullSiStr = valeurDoitEtreNullSi?.ToString();
                    if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                    {
                        if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) return;
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
                }

                void TraiterDoitEtreNullOuZeroSi(List<ErreurExcel> erreurs, donnees_brutes ligne, RegleValidation regle, Dictionary<string, PropertyInfo> propertyCache)
                {
                    if (string.IsNullOrEmpty(regle.nom_colonne)) return;
                    if (!propertyCache.TryGetValue(regle.nom_colonne, out var prop)) return;
                    object valeurDoitEtreNullOuZeroSi = null;
                    try { valeurDoitEtreNullOuZeroSi = prop.GetValue(ligne); } catch { }
                    var valeurDoitEtreNullOuZeroSiStr = valeurDoitEtreNullOuZeroSi?.ToString();
                    if (!string.IsNullOrEmpty(regle.colonne_dependante) && !string.IsNullOrEmpty(regle.valeur_dependante))
                    {
                        if (!propertyCache.TryGetValue(regle.colonne_dependante, out var propDep)) return;
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
                }
                #endregion

                private void mapperColonnesAvecTypesEnProd(List<MappingColonnes> mappings, object source, object target)
                {
                    foreach (var mapping in mappings)
                    {
                        if (string.IsNullOrEmpty(mapping.colonne_prod))
                            continue;
                        var donneeSource = source.GetType().GetProperty(mapping.colonne_bdd);
                        var donneeCible = target.GetType().GetProperty(mapping.colonne_prod);
                        if (donneeSource != null && donneeCible != null)
                        {
                            var value = donneeSource.GetValue(source);
                            if (value != null)
                            {
                                object convertedValue = value;
                                switch (mapping.type_donnee_prod?.ToLower())
                                {
                                    case "dateonly":
                                        if (DateOnly.TryParse(value.ToString(), out var dateOnlyValue))
                                            convertedValue = dateOnlyValue;
                                        else
                                            throw new Exception($"{value} : {donneeCible.Name}");
                                        break;
                                    case "datetime":
                                        if (DateTime.TryParse(value.ToString(), out var dateTimeValue))
                                            convertedValue = dateTimeValue;
                                        else
                                            throw new Exception($"{value} : {donneeCible.Name}");
                                        break;
                                    case "decimal":
                                        if (decimal.TryParse(value.ToString(), out var decimalValue))
                                            convertedValue = decimalValue;
                                        else
                                            throw new Exception($"{value} : {donneeCible.Name}");
                                        break;
                                    case "entier":
                                        if (int.TryParse(value.ToString(), out var intValue))
                                            convertedValue = intValue;
                                        else
                                            throw new Exception($"{value} : {donneeCible.Name}");
                                        break;
                                    case "bool":
                                        if (bool.TryParse(value.ToString(), out var boolValue))
                                            convertedValue = boolValue;
                                        else if (value.ToString() == "1")
                                            convertedValue = true;
                                        else if (value.ToString() == "0")
                                            convertedValue = false;
                                        else
                                            throw new Exception($"{value} : {donneeCible.Name}");
                                        break;
                                    default:
                                        convertedValue = value.ToString();
                                        break;
                                }
                                donneeCible.SetValue(target, convertedValue);
                            }
                        }
                    }
                }
    } 
            
        
}
    


    
 
 