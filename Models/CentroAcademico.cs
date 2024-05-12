using System;
using System.ComponentModel.DataAnnotations;

namespace OrientaTEC_MVC.Models
{
    public class CentroAcademico
    {
        private string iniciales;
        private string nombre;
        private bool esSedeCentral;

        [Required]
        public string Iniciales
        {
            get { return iniciales; }
            set { iniciales = value; }
        }

        [Required]
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        [Required]
        public bool EsSedeCentral
        {
            get { return esSedeCentral; }
            set { esSedeCentral = value; }
        }
    }
}

