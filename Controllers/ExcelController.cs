using DCCR_SERVER.Context;
using DCCR_SERVER.DTOs;
using DCCR_SERVER.DTOs.Excel;
using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Services.Credits;
using DCCR_SERVER.Services.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace DCCR_SERVER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly ServiceIntegration _integration;
        private readonly ILogger<ExcelController> _logger;
        private readonly ErreurExcelExportService _exportService;
        private readonly ServiceExcelCRUD _serviceExcelCRUD;
        private readonly BddContext _context;


        public ExcelController(ServiceIntegration integration, 
                               ILogger<ExcelController> logger, 
                               ErreurExcelExportService exportService,
                               ServiceExcelCRUD serviceExcelCRUD,
                                BddContext context
                                )
        {
            _integration = integration;
            _logger = logger;
            _exportService = exportService;
            _serviceExcelCRUD = serviceExcelCRUD;
            _context = context;

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

        [HttpPost("confirmer-integration")]
        public async Task<IActionResult> MigrerStagingVersProd([FromQuery] int idExcel)
        {
            try
            {
                await _integration.MigrerDonneesStagingVersProdAsync(idExcel);
                return Ok(new { success = true, message = "succès yipiiii." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la migration staging -> production");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("exporter-erreurs-excel/{idExcel}")]
        public async Task<IActionResult> ExporterErreurs(int idExcel)
        {
            try
            {
                var fichierExcel = await _context.fichiers_excel.FirstOrDefaultAsync(f => f.id_fichier_excel == idExcel);
                string nomFichier = fichierExcel.nom_fichier_excel;
                string NomFichierSansSuffixe = Path.GetFileNameWithoutExtension(nomFichier);
                var fileBytes = await _exportService.ExportEtSuppressionErreursAsync(idExcel);
                var fileName = $"erreurs_{NomFichierSansSuffixe}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'export et suppression des erreurs Excel");
                return StatusCode(500, $"Erreur lors de l'export : {ex.Message}");
            }
        }

        [HttpGet("get-tous-metadonnes-excel")]
        public async Task<ActionResult<List<ExcelMetaDonneesDto>>> getTousLesMetaDonneesDuExcel()
        {
            try
            {
                var metadonnees_excel = await _serviceExcelCRUD.getTousLesMetaDonneesDuExcel();
                return Ok(metadonnees_excel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur interne wtf.");
            }

        }


        [HttpDelete("supprimer-fichier-excel/{idExcel}")]
        public async Task<IActionResult> supprimerFichierExcel(int idExcel)
        {
            try
            {
                var resultat = await _serviceExcelCRUD.supprimerFichierExcel(idExcel);
                if (!resultat)
                    return NotFound("Fichier Excel non trouvé.");

                return Ok(new { success = true, message = "Fichier Excel supprimé avec succès." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du fichier Excel");
                return StatusCode(500, new { success = false, message = "Erreur lors de la suppression du fichier Excel." });
            }
        }
    }
}
