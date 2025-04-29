using DCCR_SERVER.DTOs;
using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Services.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace DCCR_SERVER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly ServiceIntegration _integration;
        private readonly ILogger<ExcelController> _logger;

        public ExcelController(ServiceIntegration integration, ILogger<ExcelController> logger)
        {
            _integration = integration;
            _logger = logger;
        }

        [HttpPost("importer")]
        public async Task<IActionResult> ImporterExcel([FromForm] ImportRequeteDto ird)
        {
            if (ird.fichier == null || ird.fichier.Length == 0)
                return BadRequest("Fichier non valide.");
            try
            {
                var result = await _integration.TraiterEtMettreEnAttenteFichierAsync(ird.fichier, ird.matricule_utilisateur);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'import Excel");
                return StatusCode(500, $"Erreur lors de l'import : {ex.Message}");
            }
        }

     
        [HttpPost("migrer-staging-vers-prod")]
        public async Task<IActionResult> MigrerStagingVersProd([FromQuery] int idExcel)
        {
            try
            {
                await _integration.MigrerDonneesStagingVersProdAsync(idExcel);
                return Ok(new { success = true, message = "Migration effectuée avec succès." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Migration impossible : lignes invalides.");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la migration staging -> production");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
