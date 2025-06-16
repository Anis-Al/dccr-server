namespace DCCR_SERVER.DTOs.Archives
{
    public class ArchiveDto
    {
        public int IdFichierExcel { get; set; }
        public string NomFichierExcel { get; set; }
        public string CheminFichierExcel { get; set; }
        public DateTime DateHeureIntegrationExcel { get; set; }
        public string Integrateur { get; set; }
    }
}
