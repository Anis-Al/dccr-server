using Microsoft.AspNetCore.Mvc;
using DCCR_SERVER.Services.Utilisateur;
using DCCR_SERVER.Models.Utilisateurs_audit;
using DCCR_SERVER.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;

namespace DCCR_SERVER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class utilisateursController : ControllerBase
    {
        private readonly ServiceUtilisateurCRUD _serviceUtilisateur;

        public utilisateursController(ServiceUtilisateurCRUD serviceUtilisateur)
        {
            _serviceUtilisateur = serviceUtilisateur;
        }

        [HttpGet]
        public async Task<ActionResult<List<UtilisateurDto>>> ObtenirTousLesUtilisateurs()
        {
            try
            {
                var utilisateurs = await _serviceUtilisateur.ObtenirTousLesUtilisateurs();
                return Ok(utilisateurs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [HttpPost("ajouter")]
        public async Task<ActionResult<String>> AjouterUtilisateur([FromBody] InscriptionDto idto)
        {
            try
            {
                var message = await _serviceUtilisateur.AjouterUtilisateur(idto);
                return message;
            }
            catch (Exception ex) {
                return StatusCode(500, $"{ex.Message}");
                    }
        }
        //[HttpPost("ajouter-ldap")]
        //public async Task<ActionResult<UtilisateurDto>> AjouterUtilisateurLDAP([FromBody] DemandeInscriptionLdap request)
        //{
        //    try
        //    {
        //        var utilisateurCree = await _serviceUtilisateur.AjouterUtilisateurLDAP(request.Matricule, request.Role);
        //        return CreatedAtAction(nameof(ObtenirTousLesUtilisateurs), new { matricule = utilisateurCree.matricule }, utilisateurCree);
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
        //    }
        //}

        [HttpDelete("supprimer/{matricule}")]
        public async Task<ActionResult> SupprimerUtilisateur(string matricule)
        {
            try
            {
                var resultat = await _serviceUtilisateur.SupprimerUtilisateur(matricule);
                if (!resultat)
                {
                    return NotFound($"Aucun utilisateur trouvé avec le matricule: {matricule}");
                }
                return Ok($"Utilisateur avec le matricule {matricule} supprimé avec succès");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }
    }
}
