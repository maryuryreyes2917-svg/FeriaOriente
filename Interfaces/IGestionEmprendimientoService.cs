
using FeriaOriente.API.DTO;

namespace FeriaOriente.API.Interfaces
{
    public interface IGestionEmprendimientoService
    {
        Task<EmprendimientoDTO> ConsultarMiEmprendimientoAsync(int emprendedorId);

        Task<EmprendimientoDTO> RegistrarEmprendimientoAsync(
            int emprendedorId, EmprendimientoDTO datos);

        Task<bool> ActualizarEmprendimientoAsync(
            int emprendedorId, EmprendimientoDTO datos);

        Task<bool> EliminarEmprendimientoAsync(int emprendedorId);

        Task<IEnumerable<ProductoDTO>> ConsultarProductosAsync(int emprendedorId);

        Task<bool> RegistrarProductoAsync(int emprendedorId, ProductoDTO datos);

        Task<bool> ActualizarProductoAsync(
            int emprendedorId, int productoId, ProductoDTO datos);

        Task<bool> EliminarProductoAsync(int emprendedorId, int productoId);

        Task<IEnumerable<ServicioDTO>> ConsultarServiciosAsync(int emprendedorId);

        Task<bool> RegistrarServicioAsync(int emprendedorId, ServicioDTO datos);

        Task<bool> ActualizarServicioAsync(
            int emprendedorId, int servicioId, ServicioDTO datos);

        Task<bool> EliminarServicioAsync(int emprendedorId, int servicioId);

        // Gestión de perfiles por el administrador
        Task<IEnumerable<EmprendedorRespuestaDTO>> ConsultarTodosLosEmprendedoresAsync();

        Task<bool> ActualizarPerfilEmprendedorAsync(
            int emprendedorId, EmprendedorRegistroDTO datos);

        Task<bool> EliminarPerfilEmprendedorAsync(int emprendedorId);

        // Consultas públicas
        Task<IEnumerable<EmprendimientoDTO>> ConsultarEmprendimientosPublicosAsync();

        Task<IEnumerable<ProductoDTO>> ConsultarProductosPublicosAsync(
            int emprendimientoId);

        Task<IEnumerable<ServicioDTO>> ConsultarServiciosPublicosAsync(
            int emprendimientoId);

        Task<EmprendedorRespuestaDTO> CrearPerfilEmprendedorAsync(
            EmprendedorRegistroDTO datos);

        // Imagen del emprendimiento
        Task<bool> ActualizarImagenEmprendimientoAsync(
            int emprendedorId, string imagenUrl);
    }
}
