namespace FeriaOriente.API.Modelos
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }

        public int EmprendimientoId { get; set; }
        public Emprendimiento Emprendimiento { get; set; }
    }
}