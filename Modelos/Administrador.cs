namespace FeriaOriente.API.Modelos
{
    public class Administrador
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string ContrasenaHash { get; set; }
    }
}