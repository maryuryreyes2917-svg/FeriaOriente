using FeriaOriente.API.DTO;

namespace FeriaOriente.API.Interfaces
{
    public interface IAuthService
    {
        Task<EmprendedorRespuestaDTO> RegistrarEmprendedorAsync(
            EmprendedorRegistroDTO datos);

        Task<string> IniciarSesionAsync(LoginDTO datos);
    }
}