namespace DCCR_SERVER.Services.Utilisateur
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Net.Mail;
    using DCCR_SERVER.Context;
    using Microsoft.EntityFrameworkCore;
    using DCCR_SERVER.Models.Utilisateurs_audit;
    using System.Security.Claims;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using static DCCR_SERVER.Models.enums.Enums;
    using DCCR_SERVER.DTOs.Auth;

    public class AuthentificationService
    {
        private readonly BddContext _context;
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;

        public AuthentificationService(BddContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _smtpClient = new SmtpClient(_configuration["SmtpSettings:Server"])
            {
                Port = int.Parse(_configuration["SmtpSettings:Port"]),
                Credentials = new System.Net.NetworkCredential(
                    _configuration["SmtpSettings:Username"],
                    _configuration["SmtpSettings:Password"]
                ),
                EnableSsl = true
            };
        }

        // insription
        public async Task<(bool success, string message)> inscrireUtilisateur(RegisterDto registerDto)
        {
            if (await _context.utilisateurs.AnyAsync(u => u.matricule == registerDto.matricule))
                return (false, "User already exists");

            string mdpGenereBrute = genererMDP();
            string mdpHashe = hasherMDP(mdpGenereBrute);

            var user = new Utilisateur
            {
                matricule = registerDto.matricule,
                nom_complet = registerDto.nom_complet,
                mot_de_passe = mdpHashe,
                role = Enum.Parse<RoleUtilisateur>(registerDto.role, true),
                email = registerDto.email
            };

            try
            {
                _context.utilisateurs.Add(user);
                await _context.SaveChangesAsync();

                await envoyerInfosParEmail(user.matricule, user.nom_complet, user.email, mdpGenereBrute);
                
                return (true, "marche");
            }
            catch (Exception ex)
            {
                return (false, $"{ex.Message}");
            }
        }
        private string genererMDP()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private async Task envoyerInfosParEmail(string matricule, string nomComplet, string email, string mdp)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["SmtpSettings:FromEmail"]),
                Subject = "Vos identifiants de connexion pour l'application des déclarations correctives",
                Body = $@"Bonjour {nomComplet},
                Voici vos identifiants de connexion:
                matricule: {matricule}
                Mot de passe: {mdp}
                Cordialement.
                ",
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            await _smtpClient.SendMailAsync(mailMessage);
        }
        public static string hasherMDP(string password)
        {
            using (var sha512 = SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }


        // connexion
        public async Task<LoginReponseDto> connecterUtilisateur(LoginDto loginDto)
        {
            var utilisateur = await _context.utilisateurs.FirstOrDefaultAsync(u => u.matricule == loginDto.matricule);
            
            if (utilisateur == null)
                return new LoginReponseDto { message = "Matricule non trouvé" };
            
            if (!verifierMDP(loginDto.mot_de_passe, utilisateur.mot_de_passe))
                return new LoginReponseDto { message = "Mot de passe incorrect" };

            var token = genererJWT(utilisateur);
            return new LoginReponseDto
            {
                token = token,
                matricule = utilisateur.matricule,
                nom_complet = utilisateur.nom_complet,
                role = utilisateur.role.ToString()
            };
        }
        public string genererJWT(Utilisateur user)
        {
            var cle = _configuration["JwtSettings:cle_jwt"];
            var issuer = _configuration["JwtSettings:issuer"];
            var audience = _configuration["JwtSettings:audience"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cle));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, user.matricule), 
                 new Claim(ClaimTypes.Name, user.nom_complet), 
                 new Claim(ClaimTypes.Role, user.role.ToString()) 
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public bool verifierMDP(string enteredPassword, string storedHash)
        {
            using var sha512 = SHA512.Create();
            var enteredHashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
            var enteredHashString = Convert.ToBase64String(enteredHashBytes);
            return enteredHashString == storedHash;
        }

        public async Task<(bool success, string message)> ChangerMotDePasse(changerMotDePasseDto dto)
        {
            var utilisateur = await _context.utilisateurs.FirstOrDefaultAsync(u => u.matricule == dto.matricule);
            if (utilisateur == null)
                return (false, "Utilisateur non trouvé");

            if (!verifierMDP(dto.ancienMotDePasse, utilisateur.mot_de_passe))
                return (false, "Ancien mot de passe incorrect");

            utilisateur.mot_de_passe = hasherMDP(dto.nouveauMotDePasse);
            await _context.SaveChangesAsync();
            return (true, "Mot de passe changé avec succès");
        }
    }
}
