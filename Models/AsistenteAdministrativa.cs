using System;
using System.ComponentModel.DataAnnotations;

namespace OrientaTEC_MVC.Models
{
    public class AsistenteAdministrativa
    {
        private string nombre1;
        private string nombre2;
        private string apellido1;
        private string apellido2;
        private string correo;
        private string contraseña;
        private int telCelular;
        private CentroAcademico centroAcademico;

        [Required]
        public string Nombre1
        {
            get { return nombre1; }
            set { nombre1 = value; }
        }

        public string Nombre2
        {
            get { return nombre2; }
            set { nombre2 = value; }
        }

        [Required]
        public string Apellido1
        {
            get { return apellido1; }
            set { apellido1 = value; }
        }

        public string Apellido2
        {
            get { return apellido2; }
            set { apellido2 = value; }
        }

        [Required]
        [EmailAddress]
        public string Correo
        {
            get { return correo; }
            set { correo = value; }
        }

        [Required]
        public string Contraseña
        {
            get { return contraseña; }
            set { contraseña = value; }
        }

        [Phone]
        public int TelCelular
        {
            get { return telCelular; }
            set { telCelular = value; }
        }

        [Required]
        public CentroAcademico CentroAcademico
        {
            get { return centroAcademico; }
            set { centroAcademico = value; }
        }
    }
}
