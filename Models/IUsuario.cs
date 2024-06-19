namespace OrientaTEC_MVC.Models
{
    public interface IUsuario
    {
        string Nombre1 { get; set; }
        string Nombre2 { get; set; }
        string Apellido1 { get; set; }
        string Apellido2 { get; set; }
        string Correo { get; set; }
        string HashedPassword { get; set; }
        string SaltPassword { get; set; }
        string Rol { get; set; }
        string Estado { get; set; }
    }
}
