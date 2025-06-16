using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DCCR_SERVER.Services.Archives;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using DCCR_SERVER.DTOs.Archives;

namespace DCCR_SERVER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class archivesController : ControllerBase
    {
        private readonly ServiceArchivesCRUD _archivesService;

        public archivesController(ServiceArchivesCRUD archivesService)
        {
            _archivesService = archivesService;
        }

        [HttpGet("fichiers-excel")]
        public async Task<ActionResult<IEnumerable<ArchiveDto>>> GetFichiersExcelArchives()
        {
            try
            {
                var fichiers = await _archivesService.getTousLesFichiersExcelArchives();
                return Ok(fichiers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue lors de la récupération des fichiers Excel archivés.", error = ex.Message });
            }
        }

        [HttpGet("fichiers-xml")]
        public async Task<ActionResult<IEnumerable<ArchiveFichierXmlDto>>> GetFichiersXmlArchives()
        {
            try
            {
                var fichiers = await _archivesService.getTousLesFichiersXmlArchives();
                return Ok(fichiers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue lors de la récupération des fichiers XML archivés.", error = ex.Message });
            }
        }

        [HttpGet("credits")]
        public async Task<ActionResult<IEnumerable<ArchiveCreditDto.ArchiveCreditListeDto>>> GetCreditsArchives()
        {
            try
            {
                var credits = await _archivesService.GetTousLesCreditsArchivesListe();
                return Ok(credits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue lors de la récupération de la liste des crédits archivés.", error = ex.Message });
            }
        }

       
    }
}
