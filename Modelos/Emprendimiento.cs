namespace FeriaOriente.API.Modelos
{
    public class Emprendimiento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Municipio { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        // Imagen del emprendimiento
        public string? ImagenUrl { get; set; }

        public int EmprendedorId { get; set; }
        public Emprendedor Emprendedor { get; set; }
    }
}