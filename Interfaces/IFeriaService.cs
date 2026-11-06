using FeriaOriente.API.DTO;

namespace FeriaOriente.API.Interfaces
{
    public interface IFeriaService
    {
        // Gestión de ferias por el administrador
        Task<IEnumerable<FeriaDTO>> ConsultarFeriasAsync();

        Task<bool> CrearFeriaAsync(FeriaDTO datos);

        Task<bool> ActualizarFeriaAsync(int feriaId, FeriaDTO datos);

        Task<bool> EliminarFeriaAsync(int feriaId);

        // Inscripciones de emprendedores
        Task<bool> InscribirEmprendimientoAsync(
            int emprendedorId, int feriaId);

        Task<bool> RetirarInscripcionAsync(
            int emprendedorId, int feriaId);

        Task<IEnumerable<FeriaDTO>> ConsultarFeriasInscritasAsync(
            int emprendedorId);

        // Consulta pública
        Task<IEnumerable<FeriaDTO>> ConsultarFeriasPublicasAsync();

        // Consulta pública de participantes e historial
        Task<IEnumerable<EmprendedorRespuestaDTO>> ConsultarParticipantesFeriaAsync(
            int feriaId);

        Task<IEnumerable<FeriaDTO>> ConsultarHistorialFeriasAsync();
    }
}