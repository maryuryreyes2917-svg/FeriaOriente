
using FeriaOriente.API.DTO;
using FeriaOriente.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeriaOriente.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(
            EmprendedorRegistroDTO datos)
        {
            try
            {
                var resultado =
                    await _authService.RegistrarEmprendedorAsync(datos);

                return Created("", resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("iniciar-sesion")]
        public async Task<IActionResult> IniciarSesion(
            LoginDTO datos)
        {
            try
            {
                var token =
                    await _authService.IniciarSesionAsync(datos);

                return Ok(new
                {
                    mensaje = "Inicio de sesión exitoso.",
                    token = token
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
        }
    }
}
