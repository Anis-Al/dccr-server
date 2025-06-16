using System.ComponentModel.DataAnnotations;
using DCCR_SERVER.Models.Statiques.TablesDomaines;


namespace DCCR_SERVER.Models.Principaux
{
    public class Crédit
    {
        public  string numero_contrat_credit { get; set; }
        public DateOnly date_declaration { get; set; }
        public int id_excel { get; set; }
        public bool? est_plafond_accorde { get; set; } = false;
        public string? situation_credit { get; set; }

        public DateOnly? date_octroi { get; set; }
        public DateOnly? date_rejet { get; set; }
        public DateOnly? date_expiration { get; set; }
        public DateOnly? date_execution { get; set; }

        public string? duree_initiale { get; set; }
        public string? duree_restante { get; set; }
        public int id_lieu { get; set; }

        public string type_credit { get; set; }
        public string? activite_credit { get; set; }
        public string monnaie { get; set; }
        public decimal? credit_accorde { get; set; }
        public string? id_plafond { get; set; }
        public decimal? taux { get; set; }
        public decimal? mensualite { get; set; }
        public decimal? cout_total_credit { get; set; }
        public decimal? solde_restant { get; set; }
        //retard
        public string? classe_retard { get; set; }
        public DateOnly? date_constatation { get; set; }
        public int? nombre_echeances_impayes { get; set; }
        public decimal? montant_interets_courus { get; set; }
        public decimal? montant_interets_retard { get; set; }
        public decimal? montant_capital_retard { get; set; }
        public string? motif { get; set; }

        //navigations
        public FichierExcel? excel { get; set; }
        public List<IntervenantCrédit> intervenantsCredit { get; set; } = new List<IntervenantCrédit>();
        public List<Garantie>? garanties { get; set; } = new List<Garantie>();
        public Lieu lieu { get; set; }  
        public Monnaie devise { get; set; }
        public TypeCrédit typecredit { get; set; }
        public SituationCrédit situationcredit { get; set; }
        public ClasseRetard? classeretard { get; set; }
        public ActivitéCrédit activitecredit { get; set; }
        public DuréeCrédit dureeinitiale { get; set; }
        public DuréeCrédit dureerestante { get; set; }







    }
}
