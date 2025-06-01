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
                role = u.role.ToString()
            }).ToList();
        }
        public async Task<UtilisateurDto> AjouterUtilisateur(RegisterDto registerDto)
        {
            var (success, message) = await _authentificationService.inscrireUtilisateur(registerDto);
            
            if (!success)
            {
                throw new InvalidOperationException(message);
            }

            var utilisateur = await _context.utilisateurs
                .FirstOrDefaultAsync(u => u.matricule == registerDto.matricule);

            return new UtilisateurDto
            {
                matricule = utilisateur.matricule,
                nom_complet = utilisateur.nom_complet,
                role = utilisateur.role.ToString()
            };
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

       
        
        // te3 ldap 
        private async Task<bool> VerifierUtilisateurLDAP(string matricule)
        {
            // 3eyet l gateway/ api te3 l ldap w wach lazem
            return await Task.FromResult(true); 
        }
        private async Task<(string nomComplet, byte[] motDePasse)> ObtenirInfosUtilisateurLDAP(string matricule)
        {
           // pour recuperer les infos de cet utilisateur depuis ldap sale paresseux va
            return await Task.FromResult(("Nom Complet", new byte[] { 0x00, 0x01 })); // remplaci hadi b return les vraies infos

        }
        //public async Task<UtilisateurDto> AjouterUtilisateurLDAP(string matricule, RoleUtilisateur role)
        //{
        //    if (await _context.utilisateurs.AnyAsync(u => u.matricule == matricule))
        //    {
        //        throw new InvalidOperationException("Un utilisateur avec ce matricule existe déjà.");
        //    }

        //    if (!await VerifierUtilisateurLDAP(matricule))
        //    {
        //        throw new InvalidOperationException("Utilisateur non trouvé dans l'Active Directory.");
        //    }

        //    var (nomComplet, motDePasse) = await ObtenirInfosUtilisateurLDAP(matricule);

        //    var nouvelUtilisateur = new Models.Utilisateurs_audit.Utilisateur
        //    {
        //        matricule = matricule,
        //        nom_complet = nomComplet,
        //        mot_de_passe = motDePasse,
        //        role = role
        //    };

        //    _context.utilisateurs.Add(nouvelUtilisateur);
        //    await _context.SaveChangesAsync();
            
        //    return new UtilisateurDto
        //    {
        //        matricule = nouvelUtilisateur.matricule,
        //        nom_complet = nouvelUtilisateur.nom_complet,
        //        role = nouvelUtilisateur.role.ToString()
        //    };
        //}
    
    
    
    }
}