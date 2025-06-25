using DCCR_SERVER.Context;
using DCCR_SERVER.DTOs.Auth;
using DCCR_SERVER.Models.Utilisateurs_audit;
using Microsoft.EntityFrameworkCore;
using static DCCR_SERVER.Models.enums.Enums;

namespace DCCR_SERVER.Services.Utilisateur
{
    public class ServiceUtilisateurCRUD
    {
        private readonly BddContext _context;        
        private readonly AuthentificationService _authentificationService;

       public ServiceUtilisateurCRUD(BddContext context, AuthentificationService authentificationService)
        {
            _context = context;
            _authentificationService = authentificationService;
        }

        //methodes crud
        public async Task<List<UtilisateurDto>> ObtenirTousLesUtilisateurs()
        {
            var utilisateurs = await _context.utilisateurs
                .AsNoTracking()
                .ToListAsync();

            return utilisateurs.Select(u => new UtilisateurDto
            {
                matricule = u.matricule,
                nom_complet = u.nom_complet,
                role = u.role.ToString(),
                email = u.email
            }).ToList();
        }
        public async Task<UtilisateurDto?> ObtenirUtilisateurParMatricule(string matricule)
        {
            
            var utilisateur = await _context.utilisateurs
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.matricule == matricule);

            if (utilisateur == null)
            {
                return null;
            }

            return new UtilisateurDto
            {
                matricule = utilisateur.matricule,
                nom_complet = utilisateur.nom_complet,
                role = utilisateur.role.ToString(),
                email = utilisateur.email
            };
        }
        public async Task<String> AjouterUtilisateur(InscriptionDto idto)
        {
            var (success, message) = await _authentificationService.inscrireUtilisateur(idto);
            
            var utilisateur = await _context.utilisateurs
                .FirstOrDefaultAsync(u => u.matricule == idto.matricule);

            return message;
        }
        public async Task<bool> SupprimerUtilisateur(string matricule)
        {
            var utilisateur = await _context.utilisateurs
                .FirstOrDefaultAsync(u => u.matricule == matricule);

            if (utilisateur == null)
            {
                return false;
            }

            _context.utilisateurs.Remove(utilisateur);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UtilisateurDto?> majUtilisateur(string matricule, UtilisateurDto utilisateurDto)
        {
            var utilisateur = await _context.utilisateurs
                .FirstOrDefaultAsync(u => u.matricule == matricule);

            if (utilisateur == null)
            {
                return null;
            }

            utilisateur.nom_complet = utilisateurDto.nom_complet;
            utilisateur.email = utilisateurDto.email;
            
            if (Enum.TryParse<RoleUtilisateur>(utilisateurDto.role, out var role))
            {
                utilisateur.role = role;
            }

            _context.utilisateurs.Update(utilisateur);
            await _context.SaveChangesAsync();

            return new UtilisateurDto
            {
                matricule = utilisateur.matricule,
                nom_complet = utilisateur.nom_complet,
                role = utilisateur.role.ToString(),
                email = utilisateur.email
            };
        }
    }
}