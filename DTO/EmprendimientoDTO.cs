namespace FeriaOriente.API.DTO
{
    public class EmprendimientoDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Municipio { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        public string? ImagenUrl { get; set; }
    }
}