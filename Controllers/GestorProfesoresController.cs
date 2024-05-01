using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace OrientaTEC_MVC.Controllers
{
    public class GestorProfesoresController : Controller
    {
        private readonly ILogger<GestorProfesoresController> _logger;
        private static List<Profesor> _profesores; // Simula una base de datos en memoria

        public GestorProfesoresController(ILogger<GestorProfesoresController> logger)
        {
            _logger = logger;
            if (_profesores == null)
            {
                _profesores = InicializarProfesores();
            }
        }

        public ActionResult GestorProfesores()
        {
            return View("~/Views/Pages/GestorProfesores.cshtml", _profesores);
        }

        private List<Profesor> InicializarProfesores()
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
                    Activo = i % 2 == 0
                });
            }
            return profesores;
        }

        [HttpPost]
        public ActionResult ToggleEstadoProfesor(string id, bool activo)
        {
            var profesor = _profesores.FirstOrDefault(p => p.Codigo == id);
            if (profesor != null)
            {
                profesor.Activo = activo;
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Profesor no encontrado" });
        }


        [HttpPost]
        public IActionResult AgregarEditarProfesor(ProfesorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Aquí lógica para agregar o actualizar el profesor
                // Puedes manejar la carga de imágenes aquí también

                // Por ahora, solo redireccionamos a la vista de gestión
                return RedirectToAction("GestorProfesores");
            }

            // Si algo falla, devolvemos a la vista con el modelo
            return View(model);
        }

    }

    public class ProfesorViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Sede { get; set; }

        [Required, MaxLength(50)]
        public string Nombre { get; set; }

        [Required, MaxLength(50)]
        public string Apellidos { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; }

        [Required, Phone]
        public string Telefono { get; set; }

        [Phone]
        public string Celular { get; set; }

        public IFormFile Foto { get; set; }
    }

}
