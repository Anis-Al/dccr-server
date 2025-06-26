using DCCR_SERVER.Context;
using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Services;
using DCCR_SERVER.Services.Décl.BA;
using DCCR_SERVER.Services.Excel;
using Microsoft.AspNetCore.Mvc;

namespace DCCR_SERVER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class declarationsBAController : ControllerBase
    {
        private readonly ServiceXmlCRUD _declBaService;
        private readonly BddContext _context;

        public declarationsBAController(ServiceXmlCRUD declBaService, BddContext context)
        {
            _declBaService = declBaService;
            _context = context;
        }

        [HttpPost("generer-declarations/{idExcel}&{matricule_utilisateur}")]
        public async Task<IActionResult> genererDeclarationsParSource(int idExcel,string matricule_utilisateur)
        {
            try
            {
                var fichierXml = await _declBaService.genererDonneesFichiersXmlAsync(idExcel,matricule_utilisateur);
                _context.fichiers_xml.Add(fichierXml);
                await _context.SaveChangesAsync();

                return Ok(new { id = fichierXml.id_fichier_xml });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("telecharger-declarations/{idXml}")]
        public async Task<IActionResult> telechargerLesFichiersParDeclarations(int idXml)
        {
            var (contenuCorrection, nomFichierCorrection, contenuSuppression, nomFichierSuppression) =
                await _declBaService.envoyerLesDonneesDeDeclarationPourTelecharger(idXml);

            if (contenuCorrection == null || contenuSuppression == null)
            {
                return NotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    var fichierCorrectionStream = archive.CreateEntry($"correction/{nomFichierCorrection}");
                    using (var streameur = fichierCorrectionStream.Open())
                    {
                        await streameur.WriteAsync(contenuCorrection, 0, contenuCorrection.Length);
                    }

                    var fichierSuppressionStream = archive.CreateEntry($"suppression/{nomFichierSuppression}");
                    using (var entryStream = fichierSuppressionStream.Open())
                    {
                        await entryStream.WriteAsync(contenuSuppression, 0, contenuSuppression.Length);
                    }
                }
                string ajrdui = DateTime.Now.ToString();
                memoryStream.Position = 0;
                return File(memoryStream.ToArray(), "application/zip", $"declarations_{ajrdui}.zip");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<FichierXml>>> tousLesDeclarationsBA()
        {
            try
            {
                var xmlFiles = await _declBaService.getTousLesFichiersXml();
                return Ok(xmlFiles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("supprimer/{idXml}")]
        public async Task<IActionResult> supprimerDeclarationBa(int idXml)
        {
            try
            {
                var resultat = await _declBaService.supprimerDeclaration(idXml);
                if (!resultat)
                    return NotFound();

                return Ok(new { success = true, message = "Déclaration Ba supprimée avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Erreur lors de la suppression du fichier Excel.\n {ex}" });
            }
        }

        [HttpPost("archiver/{idExcel}")]
        public async Task<IActionResult> archiver(int idExcel)
        {
            try
            {
                var result = await _declBaService.MigrerVersArchivesAsync(idExcel);
                if (!result)
                    return BadRequest(new { success = false, message = "La migration vers les archives a échoué." });

                return Ok(new { success = true, message = "Données migrées vers les archives avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Erreur lors de la migration vers les archives: {ex.Message}" });
            }
        }
    }
}