
using FeriaOriente.API.DTO;
using FeriaOriente.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using System.IO;

namespace FeriaOriente.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmprendimientoController : ControllerBase
    {
        private readonly IGestionEmprendimientoService _service;
        private readonly IWebHostEnvironment _environment;

        public EmprendimientoController(
            IGestionEmprendimientoService service,
            IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        private int ObtenerEmprendedorId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // ==========================================
        // EMPRENDIMIENTO DEL EMPRENDEDOR
        // ==========================================

        [Authorize(Roles = "Emprendedor")]
        [HttpGet("mi-emprendimiento")]
        public async Task<IActionResult> ConsultarMiEmprendimiento()
        {
            var resultado = await _service.ConsultarMiEmprendimientoAsync(
                ObtenerEmprendedorId());

            if (resultado == null)
                return NotFound(new { mensaje = "No tienes un emprendimiento registrado." });

            return Ok(resultado);
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpPost("mi-emprendimiento")]
        public async Task<IActionResult> RegistrarEmprendimiento(
            EmprendimientoDTO datos)
        {
            var resultado = await _service.RegistrarEmprendimientoAsync(
                ObtenerEmprendedorId(), datos);

            if (resultado == null)
                return BadRequest(new { mensaje = "No se pudo registrar el emprendimiento. Verifica si ya tienes uno registrado." });

            return Created("", resultado);
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpPut("mi-emprendimiento")]
        public async Task<IActionResult> ActualizarEmprendimiento(
            EmprendimientoDTO datos)
        {
            bool actualizado = await _service.ActualizarEmprendimientoAsync(
                ObtenerEmprendedorId(), datos);

            if (!actualizado)
                return NotFound(new { mensaje = "No se encontró tu emprendimiento." });

            return Ok(new { mensaje = "Emprendimiento actualizado correctamente." });
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpDelete("mi-emprendimiento")]
        public async Task<IActionResult> EliminarEmprendimiento()
        {
            bool eliminado = await _service.EliminarEmprendimientoAsync(
                ObtenerEmprendedorId());

            if (!eliminado)
                return NotFound(new { mensaje = "No se encontró tu emprendimiento." });

            return Ok(new { mensaje = "Emprendimiento eliminado correctamente." });
        }

        // ==========================================
        // SUBIR IMAGEN DEL EMPRENDIMIENTO
        // ==========================================

        [Authorize(Roles = "Emprendedor")]
        [HttpPost("mi-emprendimiento/imagen")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> SubirImagenEmprendimiento(
            IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0)
                return BadRequest(new { mensaje = "Debes seleccionar una imagen." });

            if (imagen.Length > 5 * 1024 * 1024)
                return BadRequest(new { mensaje = "La imagen no puede superar los 5 MB." });

            string extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

            string[] extensionesPermitidas =
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

            string[] tiposPermitidos =
            {
                "image/jpeg", "image/png", "image/webp"
            };

            if (!extensionesPermitidas.Contains(extension) ||
                !tiposPermitidos.Contains(imagen.ContentType.ToLowerInvariant()))
            {
                return BadRequest(new
                {
                    mensaje = "Formato no permitido. Usa JPG, JPEG, PNG o WEBP."
                });
            }

            string carpeta = Path.Combine(
                _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
                "imagenes",
                "emprendimientos");

            Directory.CreateDirectory(carpeta);

            string nombreArchivo = Guid.NewGuid().ToString() + extension;
            string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var archivo = new FileStream(rutaCompleta, FileMode.Create))
            {
                await imagen.CopyToAsync(archivo);
            }

            string imagenUrl = "/imagenes/emprendimientos/" + nombreArchivo;

            bool actualizado = await _service.ActualizarImagenEmprendimientoAsync(
                ObtenerEmprendedorId(), imagenUrl);

            if (!actualizado)
            {
                System.IO.File.Delete(rutaCompleta);

                return NotFound(new
                {
                    mensaje = "Primero debes registrar tu emprendimiento."
                });
            }

            return Ok(new
            {
                mensaje = "Imagen actualizada correctamente.",
                imagenUrl = imagenUrl
            });
        }

        // ==========================================
        // PRODUCTOS
        // ==========================================

        [Authorize(Roles = "Emprendedor")]
        [HttpGet("mis-productos")]
        public async Task<IActionResult> ConsultarProductos()
        {
            var resultado = await _service.ConsultarProductosAsync(
                ObtenerEmprendedorId());

            return Ok(resultado);
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpPost("mis-productos")]
        public async Task<IActionResult> RegistrarProducto(ProductoDTO datos)
        {
            bool registrado = await _service.RegistrarProductoAsync(
                ObtenerEmprendedorId(), datos);

            if (!registrado)
                return NotFound(new { mensaje = "Primero debes registrar tu emprendimiento." });

            return Ok(new { mensaje = "Producto registrado correctamente." });
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpPut("mis-productos/{productoId}")]
        public async Task<IActionResult> ActualizarProducto(
            int productoId, ProductoDTO datos)
        {
            bool actualizado = await _service.ActualizarProductoAsync(
                ObtenerEmprendedorId(), productoId, datos);

            if (!actualizado)
                return NotFound(new { mensaje = "Producto no encontrado." });

            return Ok(new { mensaje = "Producto actualizado correctamente." });
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpDelete("mis-productos/{productoId}")]
        public async Task<IActionResult> EliminarProducto(int productoId)
        {
            bool eliminado = await _service.EliminarProductoAsync(
                ObtenerEmprendedorId(), productoId);

            if (!eliminado)
                return NotFound(new { mensaje = "Producto no encontrado." });

            return Ok(new { mensaje = "Producto eliminado correctamente." });
        }

        // ==========================================
        // SERVICIOS
        // ==========================================

        [Authorize(Roles = "Emprendedor")]
        [HttpGet("mis-servicios")]
        public async Task<IActionResult> ConsultarServicios()
        {
            var resultado = await _service.ConsultarServiciosAsync(
                ObtenerEmprendedorId());

            return Ok(resultado);
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpPost("mis-servicios")]
        public async Task<IActionResult> RegistrarServicio(ServicioDTO datos)
        {
            bool registrado = await _service.RegistrarServicioAsync(
                ObtenerEmprendedorId(), datos);

            if (!registrado)
                return NotFound(new { mensaje = "Primero debes registrar tu emprendimiento." });

            return Ok(new { mensaje = "Servicio registrado correctamente." });
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpPut("mis-servicios/{servicioId}")]
        public async Task<IActionResult> ActualizarServicio(
            int servicioId, ServicioDTO datos)
        {
            bool actualizado = await _service.ActualizarServicioAsync(
                ObtenerEmprendedorId(), servicioId, datos);

            if (!actualizado)
                return NotFound(new { mensaje = "Servicio no encontrado." });

            return Ok(new { mensaje = "Servicio actualizado correctamente." });
        }

        [Authorize(Roles = "Emprendedor")]
        [HttpDelete("mis-servicios/{servicioId}")]
        public async Task<IActionResult> EliminarServicio(int servicioId)
        {
            bool eliminado = await _service.EliminarServicioAsync(
                ObtenerEmprendedorId(), servicioId);

            if (!eliminado)
                return NotFound(new { mensaje = "Servicio no encontrado." });

            return Ok(new { mensaje = "Servicio eliminado correctamente." });
        }

        // ==========================================
        // ADMINISTRADOR: PERFILES
        // ==========================================

        [Authorize(Roles = "Administrador")]
        [HttpGet("todos-los-emprendedores")]
        public async Task<IActionResult> ConsultarTodosLosEmprendedores()
        {
            var resultado = await _service.ConsultarTodosLosEmprendedoresAsync();
            return Ok(resultado);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("perfil")]
        public async Task<IActionResult> CrearPerfilEmprendedor(
            EmprendedorRegistroDTO datos)
        {
            var resultado = await _service.CrearPerfilEmprendedorAsync(datos);

            if (resultado == null)
                return BadRequest(new
                {
                    mensaje = "El correo ya se encuentra registrado."
                });

            return Created("", resultado);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("perfil/{emprendedorId}")]
        public async Task<IActionResult> ActualizarPerfilEmprendedor(
            int emprendedorId, EmprendedorRegistroDTO datos)
        {
            bool actualizado = await _service.ActualizarPerfilEmprendedorAsync(
                emprendedorId, datos);

            if (!actualizado)
                return NotFound(new { mensaje = "No se pudo actualizar el perfil. Verifica el ID y el correo." });

            return Ok(new { mensaje = "Perfil actualizado correctamente." });
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("perfil/{emprendedorId}")]
        public async Task<IActionResult> EliminarPerfilEmprendedor(int emprendedorId)
        {
            bool eliminado = await _service.EliminarPerfilEmprendedorAsync(
                emprendedorId);

            if (!eliminado)
                return NotFound(new { mensaje = "Emprendedor no encontrado." });

            return Ok(new { mensaje = "Perfil eliminado correctamente." });
        }

        // ==========================================
        // CONSULTAS PÚBLICAS
        // ==========================================

        [AllowAnonymous]
        [HttpGet("publicos")]
        public async Task<IActionResult> ConsultarEmprendimientosPublicos()
        {
            var resultado = await _service.ConsultarEmprendimientosPublicosAsync();
            return Ok(resultado);
        }

        [AllowAnonymous]
        [HttpGet("publicos/{emprendimientoId}/productos")]
        public async Task<IActionResult> ConsultarProductosPublicos(
            int emprendimientoId)
        {
            var resultado = await _service.ConsultarProductosPublicosAsync(
                emprendimientoId);

            return Ok(resultado);
        }

        [AllowAnonymous]
        [HttpGet("publicos/{emprendimientoId}/servicios")]
        public async Task<IActionResult> ConsultarServiciosPublicos(
            int emprendimientoId)
        {
            var resultado = await _service.ConsultarServiciosPublicosAsync(
                emprendimientoId);

            return Ok(resultado);
        }
    }
}
