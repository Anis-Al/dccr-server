using System.ComponentModel.DataAnnotations;
using DCCR_SERVER.Models.Statiques.TablesDomaines;

namespace DCCR_SERVER.Models.Principaux.Archives
{
    public class ArchiveCrédit
    {
        public string numero_contrat_credit { get; set; }
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
        public string? classe_retard { get; set; }
        public DateOnly? date_constatation { get; set; }
        public int? nombre_echeances_impayes { get; set; }
        public decimal? montant_interets_courus { get; set; }
        public decimal? montant_interets_retard { get; set; }
        public decimal? montant_capital_retard { get; set; }
        public string? motif { get; set; }

        // Navigation properties
        public ArchiveFichierExcel? excel { get; set; }
        public List<ArchiveIntervenantCrédit> intervenantsCredit { get; set; } = new List<ArchiveIntervenantCrédit>();
        public List<ArchiveGarantie>? garanties { get; set; } = new List<ArchiveGarantie>();
        
    }
}