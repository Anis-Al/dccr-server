namespace DCCR_SERVER.DTOs.Auth
{
    public class RegisterDto
    {
        public string matricule { get; set; }
        public string nom_complet { get; set; }
        public string mot_de_passe { get; set; }
        public string role { get; set; }
    }
}
