using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Diagnostics;

namespace OrientaTEC_MVC.Controllers
{
    public class PagesController : Controller
    {
        private readonly ILogger<PagesController> _logger;

        public PagesController(ILogger<PagesController> logger)
        {
            _logger = logger;
        }

        public IActionResult GestorEquiposGuia()
        {
            return View();
        }

        public IActionResult GestorEstudiantes()
        {
            return View();
        }


        public IActionResult MenuPrincipal()
        {
            return View();
        }

        public IActionResult PlaneacionActividades()
        {
            return View();
        }

        
    }
}
