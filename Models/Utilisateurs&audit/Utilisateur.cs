using System.ComponentModel.DataAnnotations.Schema;
using DCCR_SERVER.Models.Principaux;
using static DCCR_SERVER.Models.enums.Enums;

namespace DCCR_SERVER.Models.Utilisateurs_audit
{
    public class Utilisateur 
    {
        public required string matricule { get; set; }
        public required string nom_complet { get; set; }
        public required string mot_de_passe { get; set; } 
        public required RoleUtilisateur role { get; set; }
        
        public List<Audit>? actions_de_cet_utilisateur { get; set; } 
        public List<FichierExcel>? fichiers_excel_integres { get; set; } 
        public List<FichierXml>? fichiers_xml_générés { get; set; } 
    }
}
