using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace OrientaTEC_MVC.ViewModels
{
    public class EquipoGuiaViewModel
    {
        public List<SelectListItem> EquiposGuia { get; set; } = new List<SelectListItem>();
        public string SelectedEquipoGuia { get; set; }

        // Constructor para inicializar la lista
        public EquipoGuiaViewModel()
        {
            EquiposGuia = new List<SelectListItem>();
        }
    }
}
