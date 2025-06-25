using DCCR_SERVER.Context;
using DCCR_SERVER.DTOs;
using DCCR_SERVER.DTOs.Credits;
using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Models.Statiques.TablesDomaines;
using DCCR_SERVER.Models.ValidationFichiers;
using DCCR_SERVER.Services.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static DCCR_SERVER.DTOs.CreditsDto;

namespace DCCR_SERVER.Services.Credits
{
    public class ServiceCreditsCRUD
    {
        private readonly BddContext _contexte;
        private readonly ILogger<ServiceCreditsCRUD> _journal;

        public ServiceCreditsCRUD(
            BddContext contexte,
            ILogger<ServiceCreditsCRUD> journal
            )
        {
            _contexte = contexte;
            _journal = journal;
        }
        private const string DateFormat = "yyyy-mm-dd";


        public async Task<List<CreditsListeDto>> tousLesCreditsListe()
        {
            try
            {   
                var queryListe = _contexte.credits
                    .AsNoTracking();

                var creditsListes = await queryListe
                    .Select(creditListe => new CreditsListeDto
                    {
                        num_contrat_credit = creditListe.numero_contrat_credit,
                        date_declaration = creditListe.date_declaration,
                        libelle_type_credit = creditListe.typecredit.domaine,
                        libelle_activite = creditListe.activitecredit.domaine,
                        libelle_situation = creditListe.situationcredit.domaine,
                        id_excel = creditListe.id_excel,

                    })
                    .ToListAsync();

                return creditsListes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<CreditDto>> infosCredit(string num_contrat_credit,DateOnly date_declaration)
        {

            try
            {
            var query = _contexte.credits
            .AsNoTracking()
            .AsQueryable();

            var hasContractFilter = !string.IsNullOrWhiteSpace(num_contrat_credit);
            var hasDateFilter = date_declaration != default;

            if (hasContractFilter || hasDateFilter)
            {
                query = query.Where(credit => 
                (!hasContractFilter || credit.numero_contrat_credit == num_contrat_credit) &&
                (!hasDateFilter || credit.date_declaration == date_declaration)
            );
        }

                var creditsData = await query
                    .Select(credit => new CreditDto
                    {
                        num_contrat_credit = credit.numero_contrat_credit,
                        date_declaration = credit.date_declaration,
                        type_credit = credit.type_credit,
                        libelle_type_credit = credit.typecredit.domaine,
                        est_plafond_accorde = credit.est_plafond_accorde,
                        id_plafond = credit.id_plafond,
                        code_activite = credit.activite_credit,
                        libelle_activite = credit.activitecredit.domaine,
                        situation = credit.situation_credit,
                        libelle_situation = credit.situationcredit.domaine,
                        motif = credit.motif,

                        code_agence = credit.lieu.code_agence,
                        libelle_agence = credit.lieu.agence.domaine,
                        code_wilaya = credit.lieu.code_wilaya,
                        libelle_wilaya = credit.lieu.wilaya.domaine,
                        code_pays = credit.lieu.code_pays,
                        libelle_pays = credit.lieu.pays.domaine,

                        credit_accorde = credit.credit_accorde,
                        monnaie = credit.monnaie,
                        libelle_monnaie = credit.devise.domaine,
                        taux_interets = credit.taux,
                        cout_total_credit = credit.cout_total_credit,
                        solde_restant = credit.solde_restant,
                        mensualite = credit.mensualite,

                        duree_initiale = credit.duree_initiale,
                        libelle_duree_initiale = credit.dureeinitiale.domaine,
                        duree_restante = credit.duree_restante,
                        libelle_duree_restante = credit.dureerestante.domaine,

                        classe_retard = credit.classe_retard,
                        libelle_classe_retard = credit.classeretard.domaine,
                        nombre_echeances_impayes = credit.nombre_echeances_impayes,
                        date_constatation_echeances_impayes = credit.date_constatation != null ? credit.date_constatation.Value : null,
                        montant_capital_retard = credit.montant_capital_retard,
                        montant_interets_retard = credit.montant_interets_retard,
                        montant_interets_courus = credit.montant_interets_courus,
                        date_octroi = credit.date_octroi,
                        date_expiration = credit.date_expiration,
                        date_execution = credit.date_execution != null ? credit.date_execution.Value : null,
                        date_rejet = credit.date_rejet != null ? credit.date_rejet.Value : null,
                        id_excel = credit.id_excel,

                        intervenants = credit.intervenantsCredit
                                         .Where(ic => ic.intervenant != null)
                                         .Select(ic => new IntervenantDto
                                         {
                                             cle = ic.intervenant.cle,
                                             type_cle = ic.intervenant.type_cle,
                                             nif = ic.intervenant.nif,
                                             rib = ic.intervenant.rib,
                                             cli = ic.intervenant.cli,
                                             niveau_responsabilite = ic.niveau_responsabilite,
                                             libelle_niveau_responsabilite = ic.niveau_resp.domaine,
                                         }).ToList(),

                        garanties = credit.garanties
                                          .Select(g => new GarantieDto
                                          {
                                              type_garantie = g.type_garantie,
                                              libelle_type_garantie = g.typeGarantie.domaine,
                                              montant_garantie = g.montant_garantie,
                                              cle_intervenant = g.guarant.cle
                                          }).ToList()
                    })
                    .ToListAsync();

                return creditsData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TablesDomainesDto>> GetToutesLesTablesDomaines()
        {
            var result = new List<TablesDomainesDto>();

            result.Add(await GetTableDomaineAsync<Agence>("agences"));
            result.Add(await GetTableDomaineAsync<Wilaya>("wilayas"));
            result.Add(await GetTableDomaineAsync<Pays>("pays"));
            result.Add(await GetTableDomaineAsync<TypeCrédit>("types_credit"));
            result.Add(await GetTableDomaineAsync<ActivitéCrédit>("activites_credit"));
            result.Add(await GetTableDomaineAsync<SituationCrédit>("situations_credit"));
            result.Add(await GetTableDomaineAsync<Monnaie>("monnaies"));
            result.Add(await GetTableDomaineAsync<DuréeCrédit>("durees_credit"));
            result.Add(await GetTableDomaineAsync<ClasseRetard>("classes_retard"));
            result.Add(await GetTableDomaineAsync<TypeGarantie>("types_garantie"));
            result.Add(await GetTableDomaineAsync<NiveauResponsabilité>("niveaux_responsabilite"));

            return result;
        }
        private async Task<TablesDomainesDto> GetTableDomaineAsync<T>(string nomTable) where T : BaseTG
        {
            var query = _contexte.Set<T>().AsNoTracking();
            var items = await query.OrderBy(x => x.code).ToListAsync();

            return new TablesDomainesDto
            {
                nom_table = nomTable,
                valeurs = items.Select(x => new ValeursTableDomaines
                {
                    code = x.code,
                    domaine = x.domaine
                }).ToList()
            };
        }
        public async Task<List<string>> creerCreditDepuisUi(CreditDto credit)
        {
            var donnees = new List<donnees_brutes>();

            if (credit == null)
            {
                return new List<string> { "Les données du crédit sont requises." };
            }

            try
            {
                if (credit.intervenants == null || !credit.intervenants.Any())
                {
                    return new List<string> { "Au moins un intervenant est requis pour le crédit." };
                }

                var sessionId = Guid.NewGuid();

                foreach (var intervenant in credit.intervenants)
                {
                    var garantieForIntervenant = credit.garanties?.FirstOrDefault(g => g.cle_intervenant == intervenant.cle);

                    var donneeRecord = new donnees_brutes
                    {
                        numero_contrat = credit.num_contrat_credit,
                        date_declaration = credit.date_declaration?.ToString("yyyy-MM-dd"), 
                        id_import_excel = credit.id_excel,
                        type_credit = credit.type_credit,
                        id_plafond = credit.id_plafond,
                        activite_credit = credit.code_activite,
                        situation_credit = credit.situation,
                        motif = credit.motif,
                        code_agence = credit.code_agence,
                        code_wilaya = credit.code_wilaya,
                        code_pays = credit.code_pays,
                        credit_accorde = credit.credit_accorde.ToString(),
                        monnaie = credit.monnaie,
                        taux = credit.taux_interets.ToString(),
                        cout_total_credit = credit.cout_total_credit.ToString(),
                        solde_restant = credit.solde_restant.ToString(),
                        mensualite = credit.mensualite.ToString(),
                        duree_initiale = credit.duree_initiale,
                        duree_restante = credit.duree_restante,
                        classe_retard = credit.classe_retard,
                        nombre_echeances_impayes = credit.nombre_echeances_impayes.ToString(),
                        date_constatation = credit.date_constatation_echeances_impayes?.ToString("yyyy-MM-dd"), 
                        montant_capital_retard = credit.montant_capital_retard.ToString(),
                        montant_interets_retard = credit.montant_interets_retard.ToString(),
                        montant_interets_courus = credit.montant_interets_courus.ToString(),
                        date_octroi = credit.date_octroi?.ToString("yyyy-MM-dd"),   
                        date_expiration = credit.date_expiration?.ToString("yyyy-MM-dd"),  
                        date_execution = credit.date_execution?.ToString("yyyy-MM-dd"),   
                        date_rejet = credit.date_rejet?.ToString("yyyy-MM-dd"), 
                        participant_cle = intervenant.cle,
                        participant_type_cle = intervenant.type_cle,
                        participant_nif = intervenant.nif,
                        participant_rib = intervenant.rib,
                        participant_cli = intervenant.cli,
                        role_niveau_responsabilite = intervenant.niveau_responsabilite,
                        type_garantie = garantieForIntervenant?.type_garantie,
                        montant_garantie = garantieForIntervenant?.montant_garantie.ToString(),
                        ligne_original = 1,
                        est_valide = true,
                        id_session_import = sessionId
                    };
                    donnees.Add(donneeRecord);
                }

                var erreurs = await new ServiceIntegration(_contexte, null, null).ValiderAvecReglesAsync(donnees);

                if (erreurs.Any())
                {
                    return erreurs.Select(e => e.message_erreur).ToList();
                }

                using (var transaction = await _contexte.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _contexte.table_intermediaire_traitement.AddRangeAsync(donnees);
                        await _contexte.SaveChangesAsync();

                        await new ServiceIntegration(_contexte, null, null).MigrerDonneesStagingVersProdAsync(credit.id_excel);

                        await transaction.CommitAsync();
                        return new List<string>();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _journal.LogError(ex, "Database error in creerCreditDepuisUi: {Message}", ex.Message);
                        return new List<string> { "Une erreur est survenue lors de l'enregistrement du crédit." };
                    }
                }
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Error in creerCreditDepuisUi: {Message}", ex.Message);
                return new List<string> { "Une erreur inattendue est survenue." };
            }
        }

        public async Task<List<string>> SupprimerCredit(string numeroContratCredit, DateOnly dateDeclaration)
        {
            try
            {
                var credit = await _contexte.credits
                    .Include(c => c.intervenantsCredit)
                    .Include(c => c.garanties)
                    .Include(c => c.lieu)
                    .FirstOrDefaultAsync(c => c.numero_contrat_credit == numeroContratCredit && 
                                            c.date_declaration == dateDeclaration);
                if (credit == null)
                {
                    return new List<string> { "Le crédit spécifié est introuvable." };
                }

                using (var transaction = await _contexte.Database.BeginTransactionAsync())
                {
                    try
                    {
                        if (credit.intervenantsCredit != null)
                        {
                            _contexte.intervenants_credits.RemoveRange(credit.intervenantsCredit);
                        }
                        if (credit.garanties != null)
                        {
                            _contexte.garanties.RemoveRange(credit.garanties);
                        }
                        _contexte.credits.Remove(credit);

                       
                        await _contexte.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return new List<string>();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _journal.LogError(ex, "Database error in SupprimerCredit: {Message}", ex.Message);
                        return new List<string> { "Une erreur est survenue lors de la suppression du crédit." };
                    }
                }
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Error in SupprimerCredit: {Message}", ex.Message);
                return new List<string> { "Une erreur inattendue est survenue lors de la suppression du crédit." };
            }
        }

        public async Task<List<string>> ModifierCreditDepuisUi(CreditDto credit)
        {
            try
            {
                var existingCredit = await _contexte.credits
                    .Include(c => c.intervenantsCredit)
                    .ThenInclude(ic => ic.intervenant)
                    .Include(c => c.garanties)
                    .FirstOrDefaultAsync(c => c.numero_contrat_credit == credit.num_contrat_credit && 
                                            c.date_declaration == credit.date_declaration);

                if (existingCredit == null)
                {
                    return new List<string> { "Le crédit spécifié est introuvable." };
                }

                
                var donnees = new List<donnees_brutes>();
                if (credit.intervenants != null)
                {
                    foreach (var intervenant in credit.intervenants)
                    {
                        var garantieForIntervenant = credit.garanties?.FirstOrDefault(g => g.cle_intervenant == intervenant.cle);
                        
                        var donneeRecord = new donnees_brutes
                        {
                            numero_contrat = credit.num_contrat_credit,
                            date_declaration = credit.date_declaration.ToString(),
                            id_import_excel = existingCredit.id_excel,
                            type_credit = credit.type_credit,
                            id_plafond = credit.id_plafond,
                            activite_credit = credit.code_activite,
                            situation_credit = credit.situation,
                            motif = credit.motif,
                            code_agence = credit.code_agence,
                            code_wilaya = credit.code_wilaya,
                            code_pays = credit.code_pays,
                            credit_accorde = credit.credit_accorde.ToString(),
                            monnaie = credit.monnaie,
                            taux = credit.taux_interets.ToString(),
                            cout_total_credit = credit.cout_total_credit.ToString(),
                            solde_restant = credit.solde_restant.ToString(),
                            mensualite = credit.mensualite.ToString(),
                            duree_initiale = credit.duree_initiale,
                            duree_restante = credit.duree_restante,
                            classe_retard = credit.classe_retard,
                            nombre_echeances_impayes = credit.nombre_echeances_impayes.ToString(),
                            date_constatation = credit.date_constatation_echeances_impayes?.ToString(),
                            montant_capital_retard = credit.montant_capital_retard.ToString(),
                            montant_interets_retard = credit.montant_interets_retard.ToString(),
                            montant_interets_courus = credit.montant_interets_courus.ToString(),
                            date_octroi = credit.date_octroi?.ToString("yyyy-MM-dd"),
                            date_expiration = credit.date_expiration?.ToString("yyyy-MM-dd"),
                            date_execution = credit.date_execution?.ToString("yyyy-MM-dd"),
                            date_rejet = credit.date_rejet?.ToString("yyyy-MM-dd"),
                            participant_cle = intervenant.cle,
                            participant_type_cle = intervenant.type_cle,
                            participant_nif = intervenant.nif,
                            participant_rib = intervenant.rib,
                            participant_cli = intervenant.cli,
                            role_niveau_responsabilite = intervenant.niveau_responsabilite,
                            type_garantie = garantieForIntervenant?.type_garantie,
                            montant_garantie = garantieForIntervenant?.montant_garantie.ToString(),
                            ligne_original = 1,
                            est_valide = true,
                            id_session_import = Guid.NewGuid()
                        };
                        donnees.Add(donneeRecord);
                    }
                }

                var erreurs = await new ServiceIntegration(_contexte, null, null).ValiderAvecReglesAsync(donnees);
                
                if (erreurs.Any())
                {
                    return erreurs.Select(e => e.message_erreur).ToList();
                }

                existingCredit.type_credit = credit.type_credit;
                existingCredit.est_plafond_accorde = credit.est_plafond_accorde;
                existingCredit.id_plafond = credit.id_plafond;
                existingCredit.activite_credit = credit.code_activite;
                existingCredit.situation_credit = credit.situation;
                existingCredit.motif = credit.motif;            
                existingCredit.lieu.code_agence = credit.code_agence;
                existingCredit.lieu.code_wilaya = credit.code_wilaya;
                existingCredit.lieu.code_pays = credit.code_pays;
                existingCredit.credit_accorde = credit.credit_accorde;
                existingCredit.monnaie = credit.monnaie;
                existingCredit.taux = credit.taux_interets;
                existingCredit.cout_total_credit = credit.cout_total_credit;
                existingCredit.solde_restant = credit.solde_restant;
                existingCredit.mensualite = credit.mensualite;
                existingCredit.duree_initiale = credit.duree_initiale;
                existingCredit.duree_restante = credit.duree_restante;
                existingCredit.classe_retard = credit.classe_retard;
                existingCredit.nombre_echeances_impayes = credit.nombre_echeances_impayes;
                existingCredit.date_constatation = credit.date_constatation_echeances_impayes;
                existingCredit.montant_capital_retard = credit.montant_capital_retard;
                existingCredit.montant_interets_retard = credit.montant_interets_retard;
                existingCredit.montant_interets_courus = credit.montant_interets_courus;

                existingCredit.date_octroi = credit.date_octroi;
                existingCredit.date_expiration = credit.date_expiration;
                existingCredit.date_execution = credit.date_execution;
                existingCredit.date_rejet = credit.date_rejet;

                if (credit.intervenants != null)
                {
                    _contexte.intervenants_credits.RemoveRange(existingCredit.intervenantsCredit);

                    
                    foreach (var intervenantDto in credit.intervenants)
                    {
                        var intervenantCredit = new IntervenantCrédit
                        {
                            credit = existingCredit,
                            intervenant = new Intervenant
                            {
                                cle = intervenantDto.cle,
                                type_cle = intervenantDto.type_cle,
                                nif = intervenantDto.nif,
                                rib = intervenantDto.rib,
                                cli = intervenantDto.cli
                            },
                            niveau_responsabilite = intervenantDto.niveau_responsabilite
                        };
                        _contexte.intervenants_credits.Add(intervenantCredit);
                    }
                }


                if (credit.garanties != null)
                {
                    _contexte.garanties.RemoveRange(existingCredit.garanties);
                    foreach (var garantieDto in credit.garanties)
                    {
                        var garantie = new Garantie
                        {
                            credit = existingCredit,
                            type_garantie = garantieDto.type_garantie,
                            montant_garantie = (decimal)garantieDto.montant_garantie,
                            cle_interventant = garantieDto.cle_intervenant
                        };
                        _contexte.garanties.Add(garantie);
                    }
                }
                await _contexte.SaveChangesAsync();
                await new ServiceIntegration(_contexte, null, null).MigrerDonneesStagingVersProdAsync(existingCredit.id_excel); 
                return new List<string>();
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Erreur lors de la modification du crédit: {Message}", ex.Message);
                throw;
            }
        }
    }
}

