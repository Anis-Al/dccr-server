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

        //[HttpPost("confirmer")]
        //public async Task<IActionResult> ConfirmerImport([FromForm] string idExcel, [FromForm] string idUtilisateur)
        //{
        //    try
        //    {
        //        var result = await _integration.ConfirmerImportAsync(idExcel, idUtilisateur);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erreur lors de la confirmation d'import");
        //        return StatusCode(500, $"Erreur lors de la confirmation : {ex.Message}");
        //    }
        //}

        // [HttpGet("erreurs")]
        // public async Task<IActionResult> GetErreurs([FromQuery] string idExcel)
        // {
        //     try
        //     {
        //         var erreurs = await _integration.GetErreursPourFichierAsync(idExcel);
        //         return Ok(erreurs);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Erreur lors de la récupération des erreurs");
        //         return StatusCode(500, $"Erreur lors de la récupération des erreurs : {ex.Message}");
        //     }
        // }

        // [HttpGet("preview")]
        // public async Task<IActionResult> GetPreview([FromQuery] string idExcel)
        // {
        //     try
        //     {
        //         var preview = await _integration.GetPreviewPourFichierAsync(idExcel);
        //         return Ok(preview);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Erreur lors de la récupération du preview");
        //         return StatusCode(500, $"Erreur lors de la récupération du preview : {ex.Message}");
        //     }
        // }

       
    }
}
