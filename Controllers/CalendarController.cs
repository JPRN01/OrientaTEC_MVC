using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrientaTEC_MVC.ViewModels;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using OrientaTEC_MVC.Controllers;

namespace OrientaTEC_MVC.Controllers
{
    public class CalendarController : Controller
    {
    
        private readonly ActividadDAO actividadDAO;
    
    
        public CalendarController()
        {
            actividadDAO = new ActividadDAO(); 
        }
    
        [Route("Calendar/EventCalendar")]
        public IActionResult EventCalendar()
        {
            var actividades = actividadDAO.ObtenerTodasLasActividades();
    
            return View("~/Views/Pages/EventCalendar.cshtml", actividades);
    
        }
    
    }
}
