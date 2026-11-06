using Microsoft.EntityFrameworkCore;
using FeriaOriente.API.Modelos;

namespace FeriaOriente.API.Datos
{
    public class FeriaOrienteContext : DbContext
    {
        public FeriaOrienteContext(DbContextOptions<FeriaOrienteContext> options)
            : base(options)
        {
        }

        public DbSet<Emprendedor> Emprendedores { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Emprendimiento> Emprendimientos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Feria> Ferias { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Servicio>()
                .Property(s => s.Precio)
                .HasPrecision(18, 2);
        }
    }
}