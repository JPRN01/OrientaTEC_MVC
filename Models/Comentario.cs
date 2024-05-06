using System;
using System.ComponentModel.DataAnnotations;

namespace OrientaTEC_MVC.Models
{
    public class Comentario
    {
        private string mensaje;
        private DateTime fechaEmision;

        [Required]
        public string Mensaje
        {
            get { return mensaje; }
            set { mensaje = value; }
        }

        [Required]
        public DateTime FechaEmision
        {
            get { return fechaEmision; }
            set { fechaEmision = value; }
        }
    }
}

