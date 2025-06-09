using DCCR_SERVER.Models.Utilisateurs_audit;
using DCCR_SERVER.Models.ValidationFichiers;
using static DCCR_SERVER.Models.enums.Enums;

namespace DCCR_SERVER.Models.Principaux.Archives
{
    public class ArchiveFichierExcel
    {
        public int id_fichier_excel { get; set; }
        public string nom_fichier_excel { get; set; }
        public string chemin_fichier_excel { get; set; }
        public string id_integrateur_excel { get; set; }
        public DateTime date_heure_integration_excel { get; set; }
        public Guid id_session_import { get; set; }
        public StatutImport statut_import { get; set; }
        public string? message_statut { get; set; }
        public string? resume_validation { get; set; }
        public List<ArchiveCrédit> credits { get; set; } = new List<ArchiveCrédit>();
        public List<ArchiveFichierXml> fichiers_xml { get; set; } = new List<ArchiveFichierXml>();
    }
}