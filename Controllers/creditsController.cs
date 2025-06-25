using System.ComponentModel.DataAnnotations;
using DCCR_SERVER.DTOs;
using DCCR_SERVER.Models.ValidationFichiers;
using DCCR_SERVER.Services.Credits;
using Microsoft.AspNetCore.Mvc;
using static DCCR_SERVER.DTOs.CreditsDto;

namespace DCCR_SERVER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class creditsController : ControllerBase
    {
        private readonly ServiceCreditsCRUD _serviceCreditsCRUD;
        private readonly ILogger<creditsController> _logger;
        public creditsController(
            ServiceCreditsCRUD serviceCreditsCRUD,
            ILogger<creditsController> logger
            )
        {
            _serviceCreditsCRUD = serviceCreditsCRUD;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CreditsDto.CreditsListeDto>>> GetAllCredits()
        {
            try
            {
                var credits = await _serviceCreditsCRUD.tousLesCreditsListe();
                return Ok(credits);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des crédits.");
            }
        }

    //   [HttpGet("infos")]
    //    public async Task<ActionResult<List<CreditDto>>> GetCreditInfos(
    //        [FromQuery][Required(ErrorMessage = "Le numéro de contrat est requis")] string numContratCredit,
    //        [FromQuery][Required(ErrorMessage = "La date de déclaration est requise")] DateTime dateDeclaration)
    //{
    //    try
    //    {
    //        var dateOnly = DateOnly.FromDateTime(dateDeclaration);
        
    //        var credits = await _serviceCreditsCRUD.infosCredit(numContratCredit, dateOnly);
        
    //        if (credits == null || !credits.Any())
    //        {
    //            return NotFound("Aucun crédit trouvé avec les critères spécifiés.");
    //        }
        
    //        return Ok(credits);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Erreur lors de la récupération des informations de crédit");
    //        return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération des informations de crédit.");
    //    }
    //}
        [HttpGet("get-tables-domaines")]
        public async Task<ActionResult<List<DTOs.Credits.TablesDomainesDto>>> GetTablesDomaines()
        {
            try
            {
                var tablesDomaines = await _serviceCreditsCRUD.GetToutesLesTablesDomaines();
                return Ok(tablesDomaines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting domain tables");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des tables de domaines.");
            }
        }

        [HttpPost("nouveau")]
        public async Task<ActionResult<List<string>>> creerCreditUi([FromBody] CreditsDto.CreditDto credit)
        {
            try
            {
                var erreurs = await _serviceCreditsCRUD.creerCreditDepuisUi(credit);
                return Ok(erreurs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credit");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la validation du crédit.");
            }
        }

        [HttpGet("infos")]
        public async Task<ActionResult<List<CreditDto>>> GetCreditInfos(
            [FromQuery] string numContratCredit ,
            [FromQuery] DateOnly dateDeclaration )
        {
            try
            {
                var credits = await _serviceCreditsCRUD.infosCredit(numContratCredit, dateDeclaration);
        
                if (credits == null || !credits.Any())
                {
                    return NotFound("Aucun crédit trouvé avec les critères spécifiés.");
                }
        
                return Ok(credits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des informations de crédit");
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération des informations de crédit.");
            }
        }

        [HttpDelete("supprimer/{numeroContratCredit}")]
        public async Task<ActionResult> SupprimerCredit(
            [FromRoute] string numeroContratCredit,
            [FromQuery] DateOnly dateDeclaration)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numeroContratCredit))
                {
                    return BadRequest("Le numéro de contrat est requis.");
                }

                var result = await _serviceCreditsCRUD.SupprimerCredit(numeroContratCredit, dateDeclaration);

                if (result.Any())
                {
                    return BadRequest(result);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du crédit {NumeroContrat}", numeroContratCredit);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "Une erreur est survenue lors de la suppression du crédit." });
            }
        }

        [HttpPut("modifier")]
        public async Task<ActionResult<List<string>>> ModifierCredit([FromBody] CreditDto credit)
        {
            try
            {
                if (credit == null)
                {
                    return BadRequest(new List<string> { "Les données du crédit sont requises." });
                }

                var erreurs = await _serviceCreditsCRUD.ModifierCreditDepuisUi(credit);
                
                if (erreurs.Any())
                {
                    return BadRequest(erreurs);
                }

                return Ok(new List<string> { "Le crédit a été modifié avec succès." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la modification du crédit");
                return StatusCode(
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { "Une erreur est survenue lors de la modification du crédit." });
            }
        }
    }
}