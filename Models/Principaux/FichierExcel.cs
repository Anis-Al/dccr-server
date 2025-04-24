using DCCR_SERVER.Models.Utilisateurs_audit;
using DCCR_SERVER.Models.ValidationFichiers;
using static DCCR_SERVER.Models.enums.Enums;

namespace DCCR_SERVER.Models.Principaux
{
    public class FichierExcel
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

        //navigations
        public List<Crédit> credits { get; set; } = new List<Crédit>();
        public List<ErreurExcel>? erreurs { get; set; } = new List<ErreurExcel>();
        public Utilisateur integrateurExcel { get; set; }
        public List<FichierXml> fichiers_xml { get; set; } = new List<FichierXml>();
        public List<donnees_brutes> donnees_brutes { get; set; } = new List<donnees_brutes>();

        
    }
}
