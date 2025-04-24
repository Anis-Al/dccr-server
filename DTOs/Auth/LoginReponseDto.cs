namespace DCCR_SERVER.DTOs.Auth
{
    public class LoginReponseDto
    {
        public string token { get; set; }
        public string matricule { get; set; }
        public string nom_complet { get; set; }
        public string role { get; set; }
    }
}
