
using FeriaOriente.API.Datos;
using FeriaOriente.API.DTO;
using FeriaOriente.API.Interfaces;
using FeriaOriente.API.Modelos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FeriaOriente.API.Servicios
{
    public class AuthService : IAuthService
    {
        private readonly FeriaOrienteContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Emprendedor> _hasherEmprendedor;
        private readonly PasswordHasher<Administrador> _hasherAdministrador;

        public AuthService(
            FeriaOrienteContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            _hasherEmprendedor = new PasswordHasher<Emprendedor>();
            _hasherAdministrador = new PasswordHasher<Administrador>();
        }

        public async Task<EmprendedorRespuestaDTO> RegistrarEmprendedorAsync(
            EmprendedorRegistroDTO datos)
        {
            bool correoExiste = await _context.Emprendedores
                .AnyAsync(e => e.Correo == datos.Correo);

            if (correoExiste)
            {
                throw new Exception("El correo ya está registrado.");
            }

            var emprendedor = new Emprendedor
            {
                Nombre = datos.Nombre,
                Apellido = datos.Apellido,
                Correo = datos.Correo,
                Telefono = datos.Telefono
            };

            emprendedor.ContrasenaHash =
                _hasherEmprendedor.HashPassword(
                    emprendedor, datos.Contrasena);

            _context.Emprendedores.Add(emprendedor);

            await _context.SaveChangesAsync();

            return new EmprendedorRespuestaDTO
            {
                Id = emprendedor.Id,
                Nombre = emprendedor.Nombre,
                Apellido = emprendedor.Apellido,
                Correo = emprendedor.Correo,
                Telefono = emprendedor.Telefono
            };
        }

        public async Task<string> IniciarSesionAsync(LoginDTO datos)
        {
            string tipoCuenta = datos.TipoCuenta?.Trim().ToLower() ?? "";

            if (tipoCuenta == "emprendedor")
            {
                var emprendedor = await _context.Emprendedores
                    .FirstOrDefaultAsync(e => e.Correo == datos.Correo);

                if (emprendedor == null)
                    throw new Exception("Credenciales incorrectas.");

                var resultado = _hasherEmprendedor.VerifyHashedPassword(
                    emprendedor,
                    emprendedor.ContrasenaHash,
                    datos.Contrasena);

                if (resultado == PasswordVerificationResult.Failed)
                    throw new Exception("Credenciales incorrectas.");

                return GenerarToken(
                    emprendedor.Id,
                    emprendedor.Correo,
                    "Emprendedor");
            }

            if (tipoCuenta == "administrador")
            {
                var administrador = await _context.Administradores
                    .FirstOrDefaultAsync(a => a.Correo == datos.Correo);

                if (administrador == null)
                    throw new Exception("Credenciales incorrectas.");

                var resultado = _hasherAdministrador.VerifyHashedPassword(
                    administrador,
                    administrador.ContrasenaHash,
                    datos.Contrasena);

                if (resultado == PasswordVerificationResult.Failed)
                    throw new Exception("Credenciales incorrectas.");

                return GenerarToken(
                    administrador.Id,
                    administrador.Correo,
                    "Administrador");
            }

            throw new Exception("Tipo de cuenta no válido.");
        }

        private string GenerarToken(
            int id,
            string correo,
            string rol)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key!));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Email, correo),
                new Claim(ClaimTypes.Role, rol)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
