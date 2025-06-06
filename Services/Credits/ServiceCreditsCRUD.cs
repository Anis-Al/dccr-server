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
        private const string DateFormat = "yyyy-MM-dd";

        

        public async Task<List<CreditDto>> getTousLesCredits()
        {

            try
            {
                var query = _contexte.credits
                    .AsNoTracking();

                var creditsData = await query
                    .OrderByDescending(c => c.date_declaration)
                    .Select(credit => new CreditDto
                    {
                        num_contrat_credit = credit.numero_contrat_credit,
                        date_declaration = credit.date_declaration.ToString(DateFormat),
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
                        date_constatation_echeances_impayes = credit.date_constatation != null ? credit.date_constatation.Value.ToString(DateFormat) : null,
                        montant_capital_retard = credit.montant_capital_retard,
                        montant_interets_retard = credit.montant_interets_retard,
                        montant_interets_courus = credit.montant_interets_courus,
                        date_octroi = credit.date_octroi.ToString(DateFormat),
                        date_expiration = credit.date_expiration.ToString(DateFormat),
                        date_execution = credit.date_execution != null ? credit.date_execution.Value.ToString(DateFormat) : string.Empty,
                        date_rejet = credit.date_rejet != null ? credit.date_rejet.Value.ToString(DateFormat) : null,
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
            try
            {
                var donnees = new List<donnees_brutes>();

                if (credit.intervenants != null)
                {
                    foreach (var intervenant in credit.intervenants)
                    {
                        var garantieForIntervenant = credit.garanties?.FirstOrDefault(g => g.cle_intervenant == intervenant.cle);
                        
                        var donneeRecord = new donnees_brutes
                        {
                            numero_contrat = credit.num_contrat_credit,
                            date_declaration = credit.date_declaration,
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
                            date_constatation = credit.date_constatation_echeances_impayes,
                            montant_capital_retard = credit.montant_capital_retard.ToString(),
                            montant_interets_retard = credit.montant_interets_retard.ToString(),
                            montant_interets_courus = credit.montant_interets_courus.ToString(),
                            date_octroi = credit.date_octroi,
                            date_expiration = credit.date_expiration,
                            date_execution = credit.date_execution,
                            date_rejet = credit.date_rejet,
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

                _contexte.table_intermediaire_traitement.AddRange(donnees);
                await _contexte.SaveChangesAsync();

                var erreurs = await new ServiceIntegration(_contexte, null, null).ValiderAvecReglesAsync(donnees);
                
                if (!erreurs.Any())
                {
                    var id_import_excel = donnees.First().id_import_excel;
                    await new ServiceIntegration(_contexte, null, null).MigrerDonneesStagingVersProdAsync(id_import_excel);
                }

                return erreurs.Select(e => e.message_erreur).ToList();
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Error in creerCreditDepuisUi: {Message}", ex.Message);
                throw;
            }
        }

}
}
