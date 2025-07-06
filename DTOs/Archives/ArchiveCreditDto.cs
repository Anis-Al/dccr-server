

namespace DCCR_SERVER.DTOs.Archives
{
    public class ArchiveCreditDto
    {
        public class ArchiveCreditListeDto
        {
            public string? num_contrat_credit { get; set; }
            public DateOnly? date_declaration { get; set; }
            public string? libelle_type_credit { get; set; }
            public string? libelle_activite { get; set; }
            public string? libelle_situation { get; set; }
            public int id_excel { get; set; }
        }
        public class ArchiveCreditDetailsDto
        {
            internal List<ArchiveIntervenantDto> intervenants;
            internal List<ArchiveGarantieDto> garanties;

            public string? num_contrat_credit { get; set; }
            public DateOnly? date_declaration { get; set; }
            public int id_excel { get; set; }

            public string? type_credit { get; set; }
            public string? libelle_type_credit { get; set; }
            public bool? est_plafond_accorde { get; set; }
            public string? id_plafond { get; set; }
            public string? code_activite { get; set; }
            public string? libelle_activite { get; set; }
            public string? situation { get; set; }
            public string? libelle_situation { get; set; }
            public string? motif { get; set; }

            public string? code_agence { get; set; }
            public string? libelle_agence { get; set; }
            public string? code_wilaya { get; set; }
            public string? libelle_wilaya { get; set; }
            public string? code_pays { get; set; }
            public string? libelle_pays { get; set; }

            public decimal? credit_accorde { get; set; }
            public string? monnaie { get; set; }
            public string? libelle_monnaie { get; set; }
            public decimal? taux_interets { get; set; }
            public decimal? cout_total_credit { get; set; }
            public decimal? solde_restant { get; set; }
            public decimal? mensualite { get; set; }

            public string? duree_initiale { get; set; }
            public string? libelle_duree_initiale { get; set; }
            public string? duree_restante { get; set; }
            public string? libelle_duree_restante { get; set; }

            public string? classe_retard { get; set; }
            public string? libelle_classe_retard { get; set; }
            public int? nombre_echeances_impayes { get; set; }
            public DateOnly? date_constatation_echeances_impayes { get; set; }
            public decimal? montant_capital_retard { get; set; }
            public decimal? montant_interets_retard { get; set; }
            public decimal? montant_interets_courus { get; set; }

            public DateOnly? date_octroi { get; set; }
            public DateOnly? date_expiration { get; set; }
            public DateOnly? date_execution { get; set; }
            public DateOnly? date_rejet { get; set; }
            
            public List<ArchiveGarantieDto> Garanties { get; set; } = new List<ArchiveGarantieDto>();
        }

        public class ArchiveIntervenantDto
        {
            public string? cle { get; set; }
            public string? type_cle { get; set; }
            public string? nif { get; set; }
            public string? rib { get; set; }
            public string? cli { get; set; }
            public string? niveau_responsabilite { get; set; }
            public string? libelle_niveau_responsabilite { get; set; }
        }

        public class ArchiveGarantieDto
        {
            public string? type_garantie { get; set; }
            public string? libelle_type_garantie { get; set; }
            public decimal? montant_garantie { get; set; }
            public string? cle_intervenant { get; set; }
        }
    }
}
