using DCCR_SERVER.DTOs;
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur interne wtf.");
            }

        }
    
    }
}