
namespace FeriaOriente.API.DTO
{
    public class FeriaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Lugar { get; set; }
        public string Municipio { get; set; }
        public bool Estado { get; set; }
    }
}
