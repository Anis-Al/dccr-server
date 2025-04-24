namespace DCCR_SERVER.DTOs
{
    public class ImportRequeteDto
    {
        // Property for the file upload
        public IFormFile fichier { get; set; }

        // Property for the user ID
        public string matricule_utilisateur { get; set; }

    }
}
