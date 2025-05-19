using DCCR_SERVER.Context;
using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Services;
using DCCR_SERVER.Services.Décl.BA;
using Microsoft.AspNetCore.Mvc;

namespace DCCR_SERVER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class declarationsBAController : ControllerBase
    {
        private readonly ServiceXmlCRUD _xmlService;
        private readonly BddContext _context;

        public declarationsBAController(ServiceXmlCRUD xmlService, BddContext context)
        {
            _xmlService = xmlService;
            _context = context;
        }

        [HttpPost("generer-declarations/{idExcel}")]
        public IActionResult GenerateXml(int idExcel)
        {
            try
            {
                var fichierXml = _xmlService.genererDonneesFichiersXml(idExcel);

                _context.fichiers_xml.Add(fichierXml);
                _context.SaveChanges();

                return Ok(new { id = fichierXml.id_fichier_xml, correction=fichierXml.contenu_correction,suppression=fichierXml.contenu_supression });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("telecharger-declarations/{idXml}")]
        public async Task<IActionResult> DownloadBothXml(int idXml)
        {
            var (correctionContent, correctionFileName, suppressionContent, suppressionFileName) =
                await _xmlService.getLesFichiersXml(idXml);

            if (correctionContent == null || suppressionContent == null)
            {
                return NotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    var correctionEntry = archive.CreateEntry(correctionFileName);
                    using (var entryStream = correctionEntry.Open())
                    {
                        await entryStream.WriteAsync(correctionContent, 0, correctionContent.Length);
                    }

                    var suppressionEntry = archive.CreateEntry(suppressionFileName);
                    using (var entryStream = suppressionEntry.Open())
                    {
                        await entryStream.WriteAsync(suppressionContent, 0, suppressionContent.Length);
                    }
                }

                memoryStream.Position = 0;
                return File(memoryStream.ToArray(), "application/zip", $"xml_files_{idXml}.zip");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<FichierXml>>> GetAllXmlFiles()
        {
            try
            {
                var xmlFiles = await _xmlService.getTousLesFichiersXml();
                return Ok(xmlFiles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}