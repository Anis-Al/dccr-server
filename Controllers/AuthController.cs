using Microsoft.AspNetCore.Mvc;
using DCCR_SERVER.Services.Utilisateur;
using DCCR_SERVER.DTOs.Auth;

namespace DCCR_SERVER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthentificationService _authentificationService;

        public AuthController(AuthentificationService authentificationService)
        {
            _authentificationService = authentificationService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginReponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var loginResponse = await _authentificationService.connecterUtilisateur(loginDto);
                if (loginResponse == null)
                {
                    return Unauthorized("Invalid credentials");
                }
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public class HashPasswordDto
        {
            public string password { get; set; }
        }

        [HttpPost("hash-password")]
        public ActionResult<string> HashPassword([FromBody] HashPasswordDto request)
        {
            try
            {
                var hashedPassword = AuthentificationService.hasherMDP(request.password);
                return Ok(hashedPassword);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}