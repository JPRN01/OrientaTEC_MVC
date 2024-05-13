using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace OrientaTEC_MVC.ViewModels
{
    public class EquipoGuiaViewModel
    {
        public int? SelectedEquipoGuia { get; set; }
        public List<SelectListItem> EquiposGuia { get; set; } // Cambiado a List
    }
}
