namespace OrientaTEC_MVC.Models
{
    public class UsuarioDecorator : IUsuario
    {
        protected IUsuario usuarioDecorado;

        public UsuarioDecorator(IUsuario usuario)
        {
            this.usuarioDecorado = usuario;
        }

        public string Nombre1
        {
            get => usuarioDecorado.Nombre1;
            set => usuarioDecorado.Nombre1 = value;
        }

        public string Nombre2
        {
            get => usuarioDecorado.Nombre2;
            set => usuarioDecorado.Nombre2 = value;
        }

        public string Apellido1
        {
            get => usuarioDecorado.Apellido1;
            set => usuarioDecorado.Apellido1 = value;
        }

        public string Apellido2
        {
            get => usuarioDecorado.Apellido2;
            set => usuarioDecorado.Apellido2 = value;
        }

        public string Correo
        {
            get => usuarioDecorado.Correo;
            set => usuarioDecorado.Correo = value;
        }

        public string HashedPassword
        {
            get => usuarioDecorado.HashedPassword;
            set => usuarioDecorado.HashedPassword = value;
        }

        public string SaltPassword
        {
            get => usuarioDecorado.SaltPassword;
            set => usuarioDecorado.SaltPassword = value;
        }

        public string Rol
        {
            get => usuarioDecorado.Rol;
            set => usuarioDecorado.Rol = value;
        }

        public string Estado
        {
            get => usuarioDecorado.Estado;
            set => usuarioDecorado.Estado = value;
        }
    }

}
