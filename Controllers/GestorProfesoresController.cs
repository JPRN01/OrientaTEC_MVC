using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrientaTEC_MVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace OrientaTEC_MVC.Controllers
{
    public class GestorProfesoresController : Controller
    {
        private readonly ILogger<GestorProfesoresController> _logger;
        private static List<Profesor> _profesores; // Simula conexión a DAO
        private readonly ProfesorDAO _profesorDAO;

        public GestorProfesoresController(ILogger<GestorProfesoresController> logger)
        {
            _profesorDAO = new ProfesorDAO();
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
            _profesores = _profesorDAO.recuperarProfesores();//new List<Profesor>();
            /*for (int i = 1; i <= 20; i++)
            {
                _profesores.Add(new Profesor
                {
                    Sede = i % 2 == 0 ? "CA" : "SJ",
                    Nombre1 = "Nombre" + i,
                    Nombre2 = "Secundario" + i,
                    Apellido1 = "Apellido" + i,
                    Apellido2 = "Secundario" + i,
                    Codigo = (i % 2 == 0 ? "CA" : "SJ") + i.ToString("D4"),
                    Correo = "profesor" + i + "@itcr.ac.cr",
                    Contrasena = "Password" + i + "!",
                    TelOficina = "1234-5678 [ext." + (4000 + i) + "]",
                    TelCelular = 800000 + i,
                    ImagenURL = "https://example.com/image" + i + ".jpg",
                    Activo = i % 2 == 0
                });
            }*/
            return _profesores;
        }


        [HttpPost]
        public ActionResult AgregarProfesor(Profesor profesor)
        {
            profesor.Contrasena = "profe" +profesor.Nombre1;
            bool check = _profesorDAO.registrarProfesorGuia(profesor);
            _profesores = _profesorDAO.recuperarProfesores();
            if (check)
            {
                return Json(new { success = true, message = "Profesor agregado correctamente." });
            }
            else
            {
                return Json(new { success = false, message = "Error en los datos del profesor." });
            }
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

        [HttpGet]
        public IActionResult GetProfesorDetails(string id)
        {
            var profesor = _profesores.FirstOrDefault(p => p.Codigo == id);
            if (profesor == null)
            {
                return Json(new { success = false, message = "Profesor no encontrado." });
            }
            return Json(new { success = true, data = profesor });
        }

        [HttpPost]
        public IActionResult EditarProfesor(Profesor profesor)
        {
            var profesor_Editado = _profesores.FirstOrDefault(p => p.Codigo == profesor.Codigo);
            if (profesor_Editado != null)
            {
                profesor_Editado.Sede = profesor.Sede;
                profesor_Editado.Nombre1 = profesor.Nombre1;
                profesor_Editado.Nombre2 = profesor.Nombre2;
                profesor_Editado.Apellido1 = profesor.Apellido1;
                profesor_Editado.Apellido2 = profesor.Apellido2;
                profesor_Editado.Correo = profesor.Correo;
                profesor_Editado.TelOficina = profesor.TelOficina;
                profesor_Editado.TelCelular = profesor.TelCelular;
                profesor_Editado.ImagenURL = profesor.ImagenURL;

                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Profesor no encontrado." });
        }


        //public ActionResult GetUpdatedProfesoresTable()
        //{
        //    var profesores = InicializarProfesores(); 
        //    return PartialView("_ProfesoresTable", profesores);
        //}



    }

}
