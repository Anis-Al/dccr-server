using DCCR_SERVER.Services.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DCCR_SERVER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class kpisController : ControllerBase
    {
        private readonly ServiceTBD _kpisService;

        public kpisController(ServiceTBD kpisService)
        {
            _kpisService = kpisService;
        }

        [HttpGet]
        public async Task<IActionResult> GetKpiResults()
        {
            try
            {
                var results = await _kpisService.ExecuterToutesRequetes();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "erreur.");
            }
        }
    }
}
