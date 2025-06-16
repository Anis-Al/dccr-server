using DCCR_SERVER.Models.Statiques.TablesDomaines;

namespace DCCR_SERVER.Models.Principaux.Archives
{
    public class ArchiveGarantie
    {
        public int id_garantie { get; set; }
        public string? cle_interventant { get; set; }
        public string numero_contrat_credit { get; set; }
        public DateOnly date_declaration { get; set; }
        public int id_excel { get; set; }
        public string type_garantie { get; set; }
        public decimal montant_garantie { get; set; }
        
        public Intervenant? guarant { get; set; }
        public ArchiveCrédit credit { get; set; }
    }
}
