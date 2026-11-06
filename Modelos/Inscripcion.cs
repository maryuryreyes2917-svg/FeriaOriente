namespace FeriaOriente.API.Modelos
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string Estado { get; set; }

        public int EmprendimientoId { get; set; }
        public Emprendimiento Emprendimiento { get; set; }

        public int FeriaId { get; set; }
        public Feria Feria { get; set; }
    }
}