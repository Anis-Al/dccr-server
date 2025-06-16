using System.ComponentModel.DataAnnotations.Schema;
using DCCR_SERVER.Models.Statiques.TablesDomaines;

namespace DCCR_SERVER.Models.Principaux.Archives
{
    public class ArchiveIntervenantCrédit
    {
        public int id_intervenantcredit { get; set; }
        public string numero_contrat_credit { get; set; }
        public DateOnly date_declaration { get; set; }
        public int id_excel { get; set; }
        public string cle_intervenant { get; set; } 
        public string niveau_responsabilite { get; set; }
        
        // Navigation properties
        public ArchiveCrédit credit { get; set; }
        public Intervenant intervenant { get; set; }
    }
}
