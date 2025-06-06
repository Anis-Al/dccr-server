using Microsoft.AspNetCore.Mvc;
using DCCR_SERVER.Services.Utilisateur;
using DCCR_SERVER.DTOs.Auth;

namespace DCCR_SERVER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class authController : ControllerBase
    {
        private readonly AuthentificationService _authentificationService;

        public authController(AuthentificationService authentificationService)
        {
            _authentificationService = authentificationService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginReponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var loginResponse = await _authentificationService.connecterUtilisateur(loginDto);
                if (loginResponse == null)
                {
                    return Unauthorized("Connexion invalide");
                }
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("changer-mdp")]
        public async Task<IActionResult> ChangerMotDePasse([FromBody] changerMotDePasseDto dto)
        {
            var (success, message) = await _authentificationService.ChangerMotDePasse(dto);
            if (success)
                return Ok(new { message });
            else
                return BadRequest(new { message });
        }
    }
}