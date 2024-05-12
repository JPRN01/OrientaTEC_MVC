using System;
using System.ComponentModel.DataAnnotations;

namespace OrientaTEC_MVC.Models
{
    public class EstadoRegistrado
    {
        private int id;
        private string grabacionURL;
        private string evidenciaURL;
        private string justificacion;
        private EstadoActividad estado;

        [Required]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string GrabacionURL
        {
            get { return grabacionURL; }
            set { grabacionURL = value; }
        }

        public string EvidenciaURL
        {
            get { return evidenciaURL; }
            set { evidenciaURL = value; }
        }

        public string Justificacion
        {
            get { return justificacion; }
            set { justificacion = value; }
        }

        [Required]
        public EstadoActividad Estado
        {
            get { return estado; }
            set { estado = value; }
        }
    }
}
