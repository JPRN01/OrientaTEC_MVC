using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OrientaTEC_MVC.Models
{
    public class Profesor
    {

        public string Sede { get; set; } //REEMPLAZAR CON LOGICA PARA DETERMINAR SEDE

        private string codigo;
        public string Codigo
        {
            get => codigo;
            set => codigo = value;
        }

        [Required]
        private string nombre1;
        public string Nombre1
        {
            get => nombre1;
            set => nombre1 = value;
        }

        private string nombre2;
        public string Nombre2
        {
            get => nombre2;
            set => nombre2 = value;
        }

        [Required]
        private string? apellido1;
        public string Apellido1
        {
            get => apellido1;
            set => apellido1 = value;
        }

        private string apellido2;
        public string Apellido2
        {
            get => apellido2;
            set => apellido2 = value;
        }

        [Required]
        [EmailAddress]
        private string correo;
        public string Correo
        {
            get => correo;
            set => correo = value;
        }

        [Required]
        private string contrasena;
        public string Contrasena
        {
            get => contrasena;
            set => contrasena = value;
        }

        private string telOficina;
        public string TelOficina
        {
            get => telOficina;
            set => telOficina = value;
        }

        private int telCelular;
        public int TelCelular
        {
            get => telCelular;
            set => telCelular = value;
        }

        private string imagenURL;
        public string ImagenURL
        {
            get => imagenURL;
            set => imagenURL = value;
        }


        private bool activo;
        public bool Activo
        {
            get => activo;
            set => activo = value;
        }

        //        public CentroAcademico CentroAcademico { get; set; }

        private string centroAcademico { get; set; }

        public string CentroAcademico
        {
            get { return centroAcademico; }
            set { centroAcademico = value; }
        }

        public bool ValidarCorreo(string correo)
        {
            return Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public void DarDeBaja()
        {
            
        }

        public string GenerarCodigo()
        {
            return Guid.NewGuid().ToString();
        }


    }
}
