using System.ComponentModel.DataAnnotations;

namespace OrientaTEC_MVC.Models
{
    public class Estudiante
    {
        private int carnet;
        private string nombre1;
        private string nombre2;
        private string apellido1;
        private string apellido2;
        private string correo;
        private int telCelular;

        [Required]
        public int Carnet
        {
            get { return carnet; }
            set { carnet = value; }
        }

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


        public int TelCelular
        {
            get { return telCelular; }
            set { telCelular = value; }
        }


        private string centroAcademico { get; set; }

        public string CentroAcademico
        {
            get { return centroAcademico; }
            set { centroAcademico = value; }
        }
    }
}
