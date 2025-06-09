using DCCR_SERVER.Models.Utilisateurs_audit;
using static DCCR_SERVER.Models.enums.Enums;

namespace DCCR_SERVER.Models.Principaux.Archives
{
    public class ArchiveFichierXml
    {
        public int id_fichier_xml { get; set; }
        public string nom_fichier_correction { get; set; }
        public string nom_fichier_suppression { get; set; }
        public required string contenu_correction { get; set; }
        public required string contenu_suppression { get; set; }
        public string id_utilisateur_generateur_xml { get; set; }
        public DateTime date_heure_generation_xml { get; set; } = DateTime.Now;
        public int id_fichier_excel { get; set; }
        public ArchiveFichierExcel fichier_excel { get; set; }
    }
}