using System;

namespace DCCR_SERVER.Models.DTOs
{
    public class XmlDto
    {
        public int IdFichierXml { get; set; }
        public string NomFichierXml { get; set; }
        public DateTime DateHeureGenerationXml { get; set; }
        public string NomUtilisateurGenerateur { get; set; }
        public string NomFichierExcelSource { get; set; }
    }
}