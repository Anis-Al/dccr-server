using DCCR_SERVER.Models.Utilisateurs_audit;

namespace DCCR_SERVER.DTOs.Excel
{
    public class ExcelMetaDonneesDto
    {
        public int id_fichier_excel { get; set; }
        public string nom_fichier_excel { get; set; }
        public string chemin_fichier_excel { get; set; }
        public string date_heure_integration_excel { get; set; }
        public string integrateur {  get; set; } 
    }
}
