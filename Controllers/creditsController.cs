using DCCR_SERVER.DTOs;
using DCCR_SERVER.Models.ValidationFichiers;
using DCCR_SERVER.Services.Credits;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("get-tous-credits")]
        public async Task<ActionResult<List<CreditsDto.CreditDto>>> GetAllCredits()
        {
            try
            {
                var credits = await _serviceCreditsCRUD.getTousLesCredits();
                return Ok(credits);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des crédits.");
            }
        }

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
    }
}