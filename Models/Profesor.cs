namespace OrientaTEC_MVC.Models
{
    public class Profesor
    {
        public string Sede { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; } = true;
    }
}
