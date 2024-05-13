using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using OrientaTEC_MVC.Models;
using Microsoft.Identity.Client;

namespace OrientaTEC_MVC.ViewModels
{
    public class ActividadesViewModel
    {
        public List<Actividad> Actividades { get;set;} 
        public ActividadesViewModel()
        {

            Actividades = new List<Actividad>();
        }
    }
}