using System.Collections.Generic;
using System.Threading.Tasks;
using DCCR_SERVER.DTOs.Dashboard;
using DCCR_SERVER.DTOs.Credits;
using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DCCR_SERVER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class tbdController : ControllerBase
    {
        private readonly ServiceTBD _queryService;
       
        private readonly ILogger<tbdController> _logger;

        public tbdController(ServiceTBD queryService, ILogger<tbdController> logger)
        {
            _queryService = queryService;
            _logger = logger;
        }


        [HttpGet("kpis")]
        public async Task<ActionResult<IEnumerable<ResultatDTO<dynamic>>>> GetAllKpisWithResults()
        {
            try
            {
                var results = await _queryService.ExecuterToutesRequetes();
                return Ok(results);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI results");
                return StatusCode(500, "Error retrieving KPI results");
            }
        }

       
    }
}