using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Diagnostics;

namespace OrientaTEC_MVC.Controllers
{
    public class GestorProfesoresController : Controller
    {
        private readonly ILogger<GestorProfesoresController> _logger;

        public GestorProfesoresController(ILogger<GestorProfesoresController> logger)
        {
            _logger = logger;
        }

        public ActionResult GestorProfesores()
        {
            List<Profesor> profesores = ObtenerProfesores();
            if (profesores == null)
            {
                // Manejo del caso donde no hay datos disponibles
                profesores = new List<Profesor>();
            }
            return View("~/Views/Pages/GestorProfesores.cshtml", profesores);

        }


        private List<Profesor> ObtenerProfesores()
        {
            List<Profesor> profesores = new List<Profesor>();

            for (int i = 1; i <= 20; i++)
            {
                profesores.Add(new Profesor
                {
                    Sede = i % 2 == 0 ? "CA" : "SJ",
                    Nombre = "Profesor " + i,
                    Codigo = "P-" + i.ToString("D4"),
                    Correo = "profesor" + i + "@itcr.ac.cr",
                    Telefono = "1234-5678 [ext. " + (4000 + i) + "]",
                    Activo = true
                });
            }
            return profesores;
        }
    }
}
