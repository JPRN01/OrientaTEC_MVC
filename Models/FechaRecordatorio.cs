using System;
using System.ComponentModel.DataAnnotations;

namespace OrientaTEC_MVC.Models
{
    public class FechaRecordatorio
    {
        private int idFecha;
        private DateTime fecha;

        [Required]
        public int IdFecha
        {
            get { return idFecha; }
            set { idFecha = value; }
        }

        [Required]
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }
    }
}


