//namespace DCCR_SERVER.Services.Utilisateur
//{
//    using System;
//    using System.Security.Cryptography;
//    using System.Text;
//    using DCCR_SERVER.Context;
//    using Microsoft.EntityFrameworkCore;
//    using DCCR_SERVER.Models.Utilisateurs_audit;
//    using System.Security.Claims;
//    using Microsoft.IdentityModel.Tokens;
//    using System.IdentityModel.Tokens.Jwt;
//    using static DCCR_SERVER.Models.enums.Enums;
//    using DCCR_SERVER.DTOs.Auth;

//    public class AuthentificationService
//    {
//        private readonly BddContext _context;
//        private readonly IConfiguration _configuration;


//        public AuthentificationService(BddContext context, IConfiguration configuration)
//        {
//            _context = context;
//            _configuration = configuration;
//        }

//        public async Task<bool> inscrireUtilisateur(RegisterDto registerDto)
//        {
//            if (await _context.utilisateurs.AnyAsync(u => u.matricule == registerDto.matricule))
//                return false;

//            byte[] passwordHash = hasherMDP(registerDto.mot_de_passe);

//            var user = new Utilisateur  
//            {
//                matricule = registerDto.matricule,
//                nom_complet = registerDto.nom_complet,
//                mot_de_passe = passwordHash,
//                role = Enum.Parse<RoleUtilisateur>(registerDto.role, true)
//            };

//            _context.utilisateurs.Add(user);
//            await _context.SaveChangesAsync();
//            return true;
//        }

//        public async Task<LoginReponseDto> connecterUtilisateur(LoginDto loginDto)
//        {
//            var utilisateur = await _context.utilisateurs.FirstOrDefaultAsync(u => u.matricule == loginDto.matricule);
//            if (utilisateur == null || !verifierMDP(loginDto.mot_de_passe, utilisateur.mot_de_passe))
//                return null;        

//            var token = genererJWT(utilisateur);
//            return new LoginReponseDto
//            {
//                Token = token,
//                Matricule = utilisateur.matricule,
//                NomComplet = utilisateur.nom_complet,
//                Role = utilisateur.role.ToString()
//            };
//        }



//        public string genererJWT(Utilisateur user)
//        {
//            var cle = _configuration["JwtSettings:cle_jwt"];
//            var issuer = _configuration["JwtSettings:issuer"];
//            var audience = _configuration["JwtSettings:audience"];
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cle));
//            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//            {
//                 new Claim(ClaimTypes.NameIdentifier, user.matricule), 
//                 new Claim(ClaimTypes.Name, user.nom_complet), 
//                 new Claim(ClaimTypes.Role, user.role.ToString()) 
//            };

//            var token = new JwtSecurityToken(
//                issuer: issuer,
//                audience: audience,
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(2), 
//                signingCredentials: credentials
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//        private bool verifierMDP(string enteredPassword, byte[] storedHash)
//        {
//            using var sha256 = SHA256.Create();
//            var enteredHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
//            return enteredHash.SequenceEqual(storedHash); 
//        }
//        private static byte[] hasherMDP(string password)
//        {
//            using (var sha512 = SHA512.Create())
//            {
//                return sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
//            }
//        }

//    }

//}
