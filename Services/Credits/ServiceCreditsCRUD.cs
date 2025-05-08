using DCCR_SERVER.Context;
using DCCR_SERVER.DTOs;
using DCCR_SERVER.Models.Principaux;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;

namespace DCCR_SERVER.Services.Credits
{
    public class ServiceCreditsCRUD
    {
        private readonly BddContext _contexte;
        public ServiceCreditsCRUD(
            BddContext contexte
            )
        {
            _contexte = contexte;
        }
        private const string DateFormat = "yyyy-MM-dd";

        public async Task<List<CreditsDto.CreditDto>> getTousLesCredits()
        {

            try
            {
                var query = _contexte.credits
                    .AsNoTracking();

                var creditsData = await query
                    .OrderByDescending(c => c.date_declaration)
                    .Select(credit => new CreditsDto.CreditDto
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
                                         .Select(ic => new CreditsDto.IntervenantDto
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
                                          .Select(g => new CreditsDto.GarantieDto
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

    }
}
