namespace DCCR_SERVER.DTOs.Archives
{
    public class ArchiveFichierXmlDto
    {
        public int IdFichierXml { get; set; }
        public string NomFichierCorrection { get; set; }
        public string NomFichierSuppression { get; set; }
        public DateTime DateHeureGenerationXml { get; set; }
        public string NomUtilisateurGenerateur { get; set; }
        public string NomFichierExcelSource { get; set; }
    }
}
