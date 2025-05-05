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
                    .AsNoTracking()
                    .Include(c => c.devise)
                    .Include(c => c.typecredit)
                    .Include(c => c.activitecredit)
                    .Include(c => c.situationcredit)
                    .Include(c => c.dureeinitiale) 
                    .Include(c => c.dureerestante) 
                    .Include(c => c.classeretard)
                    .Include(c => c.lieu).ThenInclude(l => l.agence)
                    .Include(c => c.lieu).ThenInclude(l => l.wilaya)
                    .Include(c => c.lieu).ThenInclude(l => l.pays)
                    .Include(c => c.intervenantsCredit).ThenInclude(ic => ic.intervenant) 
                    .Include(c => c.intervenantsCredit) .ThenInclude(ic => ic.niveau_resp) 
                    .Include(c => c.garanties).ThenInclude(g => g.guarant) 
                    .Include(c => c.garanties).ThenInclude(g => g.typeGarantie)
                    ;

                var creditsData = await query
                    .OrderByDescending(c => c.date_declaration) 
                    .Select(c => MapCreditToDto(c))
                    .ToListAsync();

                return creditsData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }





        private static CreditsDto.CreditDto MapCreditToDto(Crédit credit)
        {
            return new CreditsDto.CreditDto
            {
                num_contrat_credit = credit.numero_contrat_credit ?? null,
                date_declaration = credit.date_declaration.ToString(DateFormat) ?? null,
                type_credit = credit.type_credit ?? null,
                libelle_type_credit = credit.typecredit.domaine ?? null,
                est_plafond_accorde = credit.est_plafond_accorde ?? null,
                id_plafond = credit.id_plafond ?? null,
                code_activite = credit.activite_credit ?? null,
                libelle_activite = credit.activitecredit.domaine ?? null,
                situation = credit.situation_credit ?? null,
                libelle_situation = credit.situationcredit.domaine ?? null,
                motif = credit.motif ?? null,

                code_agence = credit.lieu.code_agence ?? null,
                libelle_agence = credit.lieu.agence?.domaine ?? null,
                code_wilaya = credit.lieu.code_wilaya ?? null,
                libelle_wilaya = credit.lieu.wilaya?.domaine ?? null,
                code_pays = credit.lieu.code_pays ?? null,
                libelle_pays = credit.lieu.pays?.domaine ?? null,

                credit_accorde = credit.credit_accorde ,
                monnaie = credit.monnaie ?? null,
                libelle_monnaie = credit.devise?.domaine ?? null,
                taux_interets = credit.taux,
                cout_total_credit = credit.cout_total_credit,
                solde_restant = credit.solde_restant,
                mensualite = credit.mensualite,

                duree_initiale = credit.duree_initiale ?? null,
                libelle_duree_initiale = credit.dureeinitiale.domaine ?? null, 
                duree_restante = credit.duree_restante ?? null,
                libelle_duree_restante = credit.dureerestante.domaine ?? null, 

                classe_retard = credit.classe_retard ?? null,
                libelle_classe_retard = credit.classeretard?.domaine ?? null,
                nombre_echeances_impayes = credit.nombre_echeances_impayes,
                date_constatation_echeances_impayes = credit.date_constatation?.ToString(DateFormat),
                montant_capital_retard = credit.montant_capital_retard,
                montant_interets_retard = credit.montant_capital_retard,
                montant_interets_courus = credit.montant_interets_courus,
                date_octroi = credit.date_octroi.ToString(DateFormat),
                date_expiration = credit.date_expiration.ToString(DateFormat),
                date_execution = credit.date_execution?.ToString(DateFormat) ?? string.Empty,
                date_rejet = credit.date_rejet?.ToString(DateFormat),
                id_excel = credit.id_excel,
                intervenants = credit.intervenantsCredit 
                                 .Select(ic => MapIntervenantToDto(ic)) 
                                 .Where(i => i != null)
                                 .ToList() ?? new List<CreditsDto.IntervenantDto>(),
                garanties = credit.garanties
                                  .Select(g => MapGarantieToDto(g))
                                  .Where(gd => gd != null)
                                  .ToList() ?? new List<CreditsDto.GarantieDto>(),
            };
        }

        private static CreditsDto.IntervenantDto? MapIntervenantToDto(IntervenantCrédit? ic) 
        {
            if (ic?.intervenant == null) return null; 
            Intervenant intervenant = ic.intervenant; 

            return new CreditsDto.IntervenantDto
            {
                cle = intervenant.cle, 
                type_cle = intervenant.type_cle, 
                nif = intervenant.nif, 
                rib = intervenant.rib, 
                cli = intervenant.cli,
                niveau_responsabilite = ic.niveau_responsabilite, 
                libelle_niveau_responsabilite = ic.niveau_resp?.domaine ?? null, 
            };
        }

        private static CreditsDto.GarantieDto? MapGarantieToDto(Garantie? garantie)
        {
            if (garantie == null) return null;
            return new CreditsDto.GarantieDto
            {
                type_garantie = garantie.type_garantie,
                libelle_type_garantie = garantie.typeGarantie?.domaine ?? null,
                montant_garantie = garantie.montant_garantie,
                cle_intervenant = garantie.guarant?.cle
            };
        }

    }
}
