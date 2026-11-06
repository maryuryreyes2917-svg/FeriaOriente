
using FeriaOriente.API.DTO;
using FeriaOriente.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FeriaOriente.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeriaController : ControllerBase
    {
        private readonly IFeriaService _service;

        public FeriaController(IFeriaService service)
        {
            _service = service;
        }

        private int ObtenerEmprendedorId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        
        // GESTIÓN DE FERIAS - ADMINISTRADOR
        

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> ConsultarFerias()
        {
            var resultado = await _service.ConsultarFeriasAsync();
            return Ok(resultado);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> CrearFeria(FeriaDTO datos)
        {
            bool creada = await _service.CrearFeriaAsync(datos);

            if (!creada)
                return BadRequest(new { mensaje = "No se pudo crear la feria." });

            return Ok(new { mensaje = "Feria creada correctamente." });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{feriaId}")]
        public async Task<IActionResult> ActualizarFeria(
            int feriaId, FeriaDTO datos)
        {
            bool actualizada = await _service.ActualizarFeriaAsync(
                feriaId, datos);

            if (!actualizada)
                return NotFound(new { mensaje = "Feria no encontrada." });

            return Ok(new { mensaje = "Feria actualizada correctamente." });
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{feriaId}")]
        public async Task<IActionResult> EliminarFeria(int feriaId)
        {
            bool eliminada = await _service.EliminarFeriaAsync(feriaId);

            if (!eliminada)
                return NotFound(new { mensaje = "Feria no encontrada." });

            return Ok(new { mensaje = "Feria eliminada correctamente." });
        }

        
        // INSCRIPCIONES - EMPRENDEDOR
        

        [Authorize(Roles = "Emprendedor")]
        [HttpPost("inscribirse/{feriaId}")]
        public async Task<IActionResult> Inscribirse(int feriaId)
        {
            bool inscrito = await _service.InscribirEmprendimientoAsync(
                ObtenerEmprendedorId(), feriaId);

            if (!inscrito)
                return BadRequest(new
                {
                    mensaje = "No se pudo realizar la inscripción. Verifica que tengas un emprendimiento y que la feria esté disponible."
                });

            return Ok(new { mensaje = "Inscripción realizada correctamente." });
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpDelete("retirarse/{feriaId}")]
        public async Task<IActionResult> Retirarse(int feriaId)
        {
            bool retirado = await _service.RetirarInscripcionAsync(
                ObtenerEmprendedorId(), feriaId);

            if (!retirado)
                return NotFound(new { mensaje = "No se encontró la inscripción." });

            return Ok(new { mensaje = "Te has retirado de la feria correctamente." });
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpGet("mis-ferias")]
        public async Task<IActionResult> ConsultarMisFerias()
        {
            var resultado = await _service.ConsultarFeriasInscritasAsync(
                ObtenerEmprendedorId());

            return Ok(resultado);
        }

        
        // INSCRIPCIÓN POR PARTE DEL ADMINISTRADOR
        

        [Authorize(Roles = "Administrador")]
        [HttpPost("inscribir-emprendedor/{emprendedorId}/{feriaId}")]
        public async Task<IActionResult> InscribirEmprendedor(
            int emprendedorId, int feriaId)
        {
            bool inscrito = await _service.InscribirEmprendimientoAsync(
                emprendedorId, feriaId);

            if (!inscrito)
                return BadRequest(new
                {
                    mensaje = "No se pudo inscribir al emprendedor. Verifica que tenga un emprendimiento y que la feria esté disponible."
                });

            return Ok(new
            {
                mensaje = "Emprendedor inscrito correctamente."
            });
        }

        [AllowAnonymous]
        [HttpGet("publicas")]
        public async Task<IActionResult> ConsultarFeriasPublicas()
        {
            var resultado = await _service.ConsultarFeriasPublicasAsync();
            return Ok(resultado);
        }

        [AllowAnonymous]
        [HttpGet("{feriaId}/participantes")]
        public async Task<IActionResult> ConsultarParticipantes(int feriaId)
        {
            var resultado = await _service.ConsultarParticipantesFeriaAsync(
                feriaId);

            return Ok(resultado);
        }

        [AllowAnonymous]
        [HttpGet("historial")]
        public async Task<IActionResult> ConsultarHistorial()
        {
            var resultado = await _service.ConsultarHistorialFeriasAsync();
            return Ok(resultado);
        }
    }
}

