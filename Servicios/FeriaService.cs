

using FeriaOriente.API.Datos;
using FeriaOriente.API.DTO;
using FeriaOriente.API.Interfaces;
using FeriaOriente.API.Modelos;
using Microsoft.EntityFrameworkCore;

namespace FeriaOriente.API.Servicios
{
    public class FeriaService : IFeriaService
    {
        private readonly FeriaOrienteContext _context;

        public FeriaService(FeriaOrienteContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeriaDTO>> ConsultarFeriasAsync()
        {
            return await _context.Ferias
                .Select(f => new FeriaDTO
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Descripcion = f.Descripcion,
                    FechaInicio = f.FechaInicio,
                    FechaFin = f.FechaFin,
                    Lugar = f.Lugar,
                    Municipio = f.Municipio,
                    Estado = f.Estado
                })
                .ToListAsync();
        }

        public async Task<bool> CrearFeriaAsync(FeriaDTO datos)
        {
            var feria = new Feria
            {
                Nombre = datos.Nombre,
                Descripcion = datos.Descripcion,
                FechaInicio = datos.FechaInicio,
                FechaFin = datos.FechaFin,
                Lugar = datos.Lugar,
                Municipio = datos.Municipio,
                Estado = datos.Estado
            };

            _context.Ferias.Add(feria);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActualizarFeriaAsync(
            int feriaId, FeriaDTO datos)
        {
            var feria = await _context.Ferias.FindAsync(feriaId);

            if (feria == null)
                return false;

            feria.Nombre = datos.Nombre;
            feria.Descripcion = datos.Descripcion;
            feria.FechaInicio = datos.FechaInicio;
            feria.FechaFin = datos.FechaFin;
            feria.Lugar = datos.Lugar;
            feria.Municipio = datos.Municipio;
            feria.Estado = datos.Estado;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarFeriaAsync(int feriaId)
        {
            var feria = await _context.Ferias
                .Include(f => f.Inscripciones)
                .FirstOrDefaultAsync(f => f.Id == feriaId);

            if (feria == null)
                return false;

            _context.Ferias.Remove(feria);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> InscribirEmprendimientoAsync(
            int emprendedorId, int feriaId)
        {
            var emprendimiento = await _context.Emprendimientos
                .FirstOrDefaultAsync(e => e.EmprendedorId == emprendedorId);

            if (emprendimiento == null)
                return false;

            var feria = await _context.Ferias.FindAsync(feriaId);

            if (feria == null || !feria.Estado)
                return false;

            bool yaInscrito = await _context.Inscripciones
                .AnyAsync(i =>
                    i.EmprendimientoId == emprendimiento.Id &&
                    i.FeriaId == feriaId);

            if (yaInscrito)
                return false;

            var inscripcion = new Inscripcion
            {
                FechaInscripcion = DateTime.Now,
                Estado = "Inscrito",
                EmprendimientoId = emprendimiento.Id,
                FeriaId = feriaId
            };

            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RetirarInscripcionAsync(
            int emprendedorId, int feriaId)
        {
            var emprendimiento = await _context.Emprendimientos
                .FirstOrDefaultAsync(e => e.EmprendedorId == emprendedorId);

            if (emprendimiento == null)
                return false;

            var inscripcion = await _context.Inscripciones
                .FirstOrDefaultAsync(i =>
                    i.EmprendimientoId == emprendimiento.Id &&
                    i.FeriaId == feriaId);

            if (inscripcion == null)
                return false;

            _context.Inscripciones.Remove(inscripcion);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<FeriaDTO>> ConsultarFeriasInscritasAsync(
            int emprendedorId)
        {
            return await _context.Inscripciones
                .Where(i =>
                    i.Emprendimiento.EmprendedorId == emprendedorId)
                .Select(i => new FeriaDTO
                {
                    Id = i.Feria.Id,
                    Nombre = i.Feria.Nombre,
                    Descripcion = i.Feria.Descripcion,
                    FechaInicio = i.Feria.FechaInicio,
                    FechaFin = i.Feria.FechaFin,
                    Lugar = i.Feria.Lugar,
                    Municipio = i.Feria.Municipio,
                    Estado = i.Feria.Estado
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FeriaDTO>> ConsultarFeriasPublicasAsync()
        {
            return await _context.Ferias
                .Where(f => f.Estado)
                .Select(f => new FeriaDTO
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Descripcion = f.Descripcion,
                    FechaInicio = f.FechaInicio,
                    FechaFin = f.FechaFin,
                    Lugar = f.Lugar,
                    Municipio = f.Municipio,
                    Estado = f.Estado
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<EmprendedorRespuestaDTO>>
            ConsultarParticipantesFeriaAsync(int feriaId)
        {
            return await _context.Inscripciones
                .Where(i => i.FeriaId == feriaId)
                .Select(i => new EmprendedorRespuestaDTO
                {
                    Id = i.Emprendimiento.Emprendedor.Id,
                    Nombre = i.Emprendimiento.Emprendedor.Nombre,
                    Apellido = i.Emprendimiento.Emprendedor.Apellido,
                    Correo = i.Emprendimiento.Emprendedor.Correo,
                    Telefono = i.Emprendimiento.Emprendedor.Telefono
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FeriaDTO>> ConsultarHistorialFeriasAsync()
        {
            return await _context.Ferias
                .Where(f => f.FechaFin < DateTime.Now)
                .OrderByDescending(f => f.FechaFin)
                .Select(f => new FeriaDTO
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Descripcion = f.Descripcion,
                    FechaInicio = f.FechaInicio,
                    FechaFin = f.FechaFin,
                    Lugar = f.Lugar,
                    Municipio = f.Municipio,
                    Estado = f.Estado
                })
                .ToListAsync();
        }
    }
}
