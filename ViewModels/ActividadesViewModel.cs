using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using OrientaTEC_MVC.Models;

namespace OrientaTEC_MVC.ViewModels
{
    public class ActividadesViewModel
    {
        public List<Actividad> Actividades { get; set; }
        public ActividadDetalle DetalleActividad { get; set; }

        public ActividadesViewModel()
        {
            Actividades = new List<Actividad>();
        }

        public class ActividadDetalle
        {
            public int IdActividad { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public int Semana { get; set; }
            public bool EsVirtual { get; set; }
            public string ReunionUrl { get; set; }
            public string AficheUrl { get; set; }
            public string Estado { get; set; }
            public int TipoActividad { get; set; }
            public string Tipo { get; set; }  // Agregar esta línea


            public int? DiasPreviosParaAnunciar { get; set; }
            public int? DiasParaRecordar { get; set; }

            public string Responsables { get; set; } // Añadir esta línea

        }
    }
}
