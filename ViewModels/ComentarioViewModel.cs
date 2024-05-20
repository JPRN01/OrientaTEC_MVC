using System;

namespace OrientaTEC_MVC.ViewModels
{
    public class ComentarioViewModel
    {
        public int IdComentario { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEmision { get; set; }
        public int IdActividad { get; set; }
        public string CentroAcademico { get; set; } // Suponiendo que también necesitas esto
        public int Numero { get; set; } // ID del profesor o usuario
        public int? IdComentarioPadre { get; set; }
    }
}
