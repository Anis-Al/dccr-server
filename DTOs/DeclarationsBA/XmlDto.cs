using System;

namespace DCCR_SERVER.Models.DTOs
{
    public class XmlDto
    {
        public int IdFichierXml { get; set; }
        public string NomFichierCorrection { get; set; }
        public string NomFichierSuppression { get; set; }
        public DateTime DateHeureGenerationXml { get; set; }
        public string NomUtilisateurGenerateur { get; set; }
        public string NomFichierExcelSource { get; set; }
    }
}