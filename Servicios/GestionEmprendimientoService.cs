
using FeriaOriente.API.Datos;
using FeriaOriente.API.DTO;
using FeriaOriente.API.Interfaces;
using FeriaOriente.API.Modelos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FeriaOriente.API.Servicios
{
    public class GestionEmprendimientoService : IGestionEmprendimientoService
    {
        private readonly FeriaOrienteContext _context;
        private readonly PasswordHasher<Emprendedor> _hasher;

        public GestionEmprendimientoService(FeriaOrienteContext context)
        {
            _context = context;
            _hasher = new PasswordHasher<Emprendedor>();
        }

        // GESTIÓN DEL EMPRENDIMIENTO DEL EMPRENDEDOR

        public async Task<EmprendimientoDTO> ConsultarMiEmprendimientoAsync(int emprendedorId)
        {
            var emprendimiento = await _context.Emprendimientos
                .FirstOrDefaultAsync(e => e.EmprendedorId == emprendedorId);

            if (emprendimiento == null)
                return null!;

            return new EmprendimientoDTO
            {
                Nombre = emprendimiento.Nombre,
                Descripcion = emprendimiento.Descripcion,
                Municipio = emprendimiento.Municipio,
                Direccion = emprendimiento.Direccion,
                Telefono = emprendimiento.Telefono,
                Correo = emprendimiento.Correo,
                ImagenUrl = emprendimiento.ImagenUrl
            };
        }

        public async Task<EmprendimientoDTO> RegistrarEmprendimientoAsync(
            int emprendedorId, EmprendimientoDTO datos)
        {
            var emprendedor = await _context.Emprendedores
                .FindAsync(emprendedorId);

            if (emprendedor == null)
                return null!;

            bool yaTieneEmprendimiento = await _context.Emprendimientos
                .AnyAsync(e => e.EmprendedorId == emprendedorId);

            if (yaTieneEmprendimiento)
                return null!;

            var emprendimiento = new Emprendimiento
            {
                Nombre = datos.Nombre,
                Descripcion = datos.Descripcion,
                Municipio = datos.Municipio,
                Direccion = datos.Direccion,
                Telefono = datos.Telefono,
                Correo = datos.Correo,
                ImagenUrl = datos.ImagenUrl,
                EmprendedorId = emprendedorId
            };

            _context.Emprendimientos.Add(emprendimiento);
            await _context.SaveChangesAsync();

            return new EmprendimientoDTO
            {
                Nombre = emprendimiento.Nombre,
                Descripcion = emprendimiento.Descripcion,
                Municipio = emprendimiento.Municipio,
                Direccion = emprendimiento.Direccion,
                Telefono = emprendimiento.Telefono,
                Correo = emprendimiento.Correo,
                ImagenUrl = emprendimiento.ImagenUrl
            };
        }

        public async Task<bool> ActualizarEmprendimientoAsync(
            int emprendedorId, EmprendimientoDTO datos)
        {
            var emprendimiento = await _context.Emprendimientos
                .FirstOrDefaultAsync(e => e.EmprendedorId == emprendedorId);

            if (emprendimiento == null)
                return false;

            emprendimiento.Nombre = datos.Nombre;
            emprendimiento.Descripcion = datos.Descripcion;
            emprendimiento.Municipio = datos.Municipio;
            emprendimiento.Direccion = datos.Direccion;
            emprendimiento.Telefono = datos.Telefono;
            emprendimiento.Correo = datos.Correo;

            if (datos.ImagenUrl != null)
                emprendimiento.ImagenUrl = datos.ImagenUrl;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarEmprendimientoAsync(int emprendedorId)
        {
            var emprendimiento = await _context.Emprendimientos
                .FirstOrDefaultAsync(e => e.EmprendedorId == emprendedorId);

            if (emprendimiento == null)
                return false;

            _context.Emprendimientos.Remove(emprendimiento);
            await _context.SaveChangesAsync();

            return true;
        }

        // GESTIÓN DE PRODUCTOS

        public async Task<IEnumerable<ProductoDTO>> ConsultarProductosAsync(int emprendedorId)
        {
            return await _context.Productos
                .Where(p => p.Emprendimiento.EmprendedorId == emprendedorId)
                .Select(p => new ProductoDTO
                {
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock
                })
                .ToListAsync();
        }

        public async Task<bool> RegistrarProductoAsync(
            int emprendedorId, ProductoDTO datos)
        {
            var emprendimiento = await _context.Emprendimientos
                .FirstOrDefaultAsync(e => e.EmprendedorId == emprendedorId);

            if (emprendimiento == null)
                return false;

            var producto = new Producto
            {
                Nombre = datos.Nombre,
                Descripcion = datos.Descripcion,
                Precio = datos.Precio,
                Stock = datos.Stock,
                EmprendimientoId = emprendimiento.Id
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActualizarProductoAsync(
            int emprendedorId, int productoId, ProductoDTO datos)
        {
            var producto = await _context.Productos
                .Include(p => p.Emprendimiento)
                .FirstOrDefaultAsync(p => p.Id == productoId);

            if (producto == null ||
                producto.Emprendimiento.EmprendedorId != emprendedorId)
                return false;

            producto.Nombre = datos.Nombre;
            producto.Descripcion = datos.Descripcion;
            producto.Precio = datos.Precio;
            producto.Stock = datos.Stock;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarProductoAsync(
            int emprendedorId, int productoId)
        {
            var producto = await _context.Productos
                .Include(p => p.Emprendimiento)
                .FirstOrDefaultAsync(p => p.Id == productoId);

            if (producto == null ||
                producto.Emprendimiento.EmprendedorId != emprendedorId)
                return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return true;
        }

        // GESTIÓN DE SERVICIOS

        public async Task<IEnumerable<ServicioDTO>> ConsultarServiciosAsync(int emprendedorId)
        {
            return await _context.Servicios
                .Where(s => s.Emprendimiento.EmprendedorId == emprendedorId)
                .Select(s => new ServicioDTO
                {
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio
                })
                .ToListAsync();
        }

        public async Task<bool> RegistrarServicioAsync(
            int emprendedorId, ServicioDTO datos)
        {
            var emprendimiento = await _context.Emprendimientos
                .FirstOrDefaultAsync(e => e.EmprendedorId == emprendedorId);

            if (emprendimiento == null)
                return false;

            var servicio = new Servicio
            {
                Nombre = datos.Nombre,
                Descripcion = datos.Descripcion,
                Precio = datos.Precio,
                EmprendimientoId = emprendimiento.Id
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActualizarServicioAsync(
            int emprendedorId, int servicioId, ServicioDTO datos)
        {
            var servicio = await _context.Servicios
                .Include(s => s.Emprendimiento)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null ||
                servicio.Emprendimiento.EmprendedorId != emprendedorId)
                return false;

            servicio.Nombre = datos.Nombre;
            servicio.Descripcion = datos.Descripcion;
            servicio.Precio = datos.Precio;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarServicioAsync(
            int emprendedorId, int servicioId)
        {
            var servicio = await _context.Servicios
                .Include(s => s.Emprendimiento)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null ||
                servicio.Emprendimiento.EmprendedorId != emprendedorId)
                return false;

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            return true;
        }

        // GESTIÓN DE PERFILES POR EL ADMINISTRADOR

        public async Task<EmprendedorRespuestaDTO> CrearPerfilEmprendedorAsync(
            EmprendedorRegistroDTO datos)
        {
            bool correoExiste = await _context.Emprendedores
                .AnyAsync(e => e.Correo == datos.Correo);

            if (correoExiste)
                return null!;

            var emprendedor = new Emprendedor
            {
                Nombre = datos.Nombre,
                Apellido = datos.Apellido,
                Correo = datos.Correo,
                Telefono = datos.Telefono
            };

            emprendedor.ContrasenaHash = _hasher.HashPassword(
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

        public async Task<IEnumerable<EmprendedorRespuestaDTO>> ConsultarTodosLosEmprendedoresAsync()
        {
            return await _context.Emprendedores
                .Select(e => new EmprendedorRespuestaDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Correo = e.Correo,
                    Telefono = e.Telefono
                })
                .ToListAsync();
        }

        public async Task<bool> ActualizarPerfilEmprendedorAsync(
            int emprendedorId, EmprendedorRegistroDTO datos)
        {
            var emprendedor = await _context.Emprendedores
                .FindAsync(emprendedorId);

            if (emprendedor == null)
                return false;

            bool correoExiste = await _context.Emprendedores
                .AnyAsync(e => e.Correo == datos.Correo &&
                               e.Id != emprendedorId);

            if (correoExiste)
                return false;

            emprendedor.Nombre = datos.Nombre;
            emprendedor.Apellido = datos.Apellido;
            emprendedor.Correo = datos.Correo;
            emprendedor.Telefono = datos.Telefono;

            if (!string.IsNullOrWhiteSpace(datos.Contrasena))
            {
                emprendedor.ContrasenaHash = _hasher.HashPassword(
                    emprendedor, datos.Contrasena);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarPerfilEmprendedorAsync(int emprendedorId)
        {
            var emprendedor = await _context.Emprendedores
                .FindAsync(emprendedorId);

            if (emprendedor == null)
                return false;

            _context.Emprendedores.Remove(emprendedor);
            await _context.SaveChangesAsync();

            return true;
        }

        // CONSULTAS PÚBLICAS

        public async Task<IEnumerable<EmprendimientoDTO>> ConsultarEmprendimientosPublicosAsync()
        {
            return await _context.Emprendimientos
                .Select(e => new EmprendimientoDTO
                {
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    Municipio = e.Municipio,
                    Direccion = e.Direccion,
                    Telefono = e.Telefono,
                    Correo = e.Correo,
                    ImagenUrl = e.ImagenUrl
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductoDTO>> ConsultarProductosPublicosAsync(
            int emprendimientoId)
        {
            return await _context.Productos
                .Where(p => p.EmprendimientoId == emprendimientoId)
                .Select(p => new ProductoDTO
                {
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ServicioDTO>> ConsultarServiciosPublicosAsync(
            int emprendimientoId)
        {
            return await _context.Servicios
                .Where(s => s.EmprendimientoId == emprendimientoId)
                .Select(s => new ServicioDTO
                {
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio
                })
                .ToListAsync();
        }

        // ACTUALIZAR IMAGEN DEL EMPRENDIMIENTO

        public async Task<bool> ActualizarImagenEmprendimientoAsync(
            int emprendedorId, string imagenUrl)
        {
            var emprendimiento = await _context.Emprendimientos
                .FirstOrDefaultAsync(e => e.EmprendedorId == emprendedorId);

            if (emprendimiento == null)
                return false;

            emprendimiento.ImagenUrl = imagenUrl;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
