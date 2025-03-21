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
                _profesores = _profesorDAO.recuperarProfesores();
            }
        }

        public ActionResult GestorProfesores()
        {
            return View("~/Views/Pages/GestorProfesores.cshtml", _profesores);
        }


        /// <summary>
        /// Codigo necesario para agregar un nuevo profesor al sistema
        /// </summary>
        /// <param name="profesor"></param>
        /// <returns></returns>
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



        /// <summary>
        /// Se usa para cambiar entre estados en un profesor, si estaba activo se pasa a desactivado y viceversa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="activo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CambiarEntreEstadosProfesor(string id, bool activo)
        {
            var profesor = _profesores.FirstOrDefault(p => p.Codigo == id);
            if (profesor != null)
            {
                profesor.Activo = activo;
                _profesorDAO.darBajaProfesorGuia(id, activo);
                return Json(new { success = true, message = "Estado de profesor cambiado" });
            }
            return Json(new { success = false, message = "Profesor no encontrado" });
        }


        /// <summary>
        /// Necesario para poder editar un profesor ya que retorna los valores incicales para editar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetProfesorDetalles(string id)
        {
            var profesor = _profesores.FirstOrDefault(p => p.Codigo == id);
            if (profesor == null)
            {
                return Json(new { success = false, message = "Profesor no encontrado." });
            }
            return Json(new { success = true, data = profesor });
        }


        /// <summary>
        /// Recibe los datos de un profesor para actualizarlo en el sistema
        /// </summary>
        /// <param name="profesor"></param>
        /// <returns></returns>
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
                bool check = _profesorDAO.modificarProfesorGuia(profesor_Editado);
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
            return Json(new { success = false, message = "Profesor no encontrado." });
        }


        //public ActionResult GetUpdatedProfesoresTable()
        //{
        //    var profesores = InicializarProfesores(); 
        //    return PartialView("_ProfesoresTable", profesores);
        //}



    }

}
