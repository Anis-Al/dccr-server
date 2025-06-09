using static DCCR_SERVER.Models.enums.Enums;

namespace DCCR_SERVER.DTOs.Auth
{
    public class InscriptionLdapDto
    {
        public required string Matricule { get; set; }
        public required RoleUtilisateur Role { get; set; }
    }
}