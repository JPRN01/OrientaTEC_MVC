namespace OrientaTEC_MVC.Models
{
    public class Usuario
    {
        public string Nombre1 { get; set; }
        public string Nombre2 { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Correo { get; set; }
        public string HashedPassword { get; set; }
        public string SaltPassword { get; set; }
        public string Rol { get; set; }
    }
}
