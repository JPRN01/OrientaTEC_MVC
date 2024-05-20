using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace OrientaTEC_MVC.Controllers
{
    public class GestorEquiposGuiaController : Controller
    {
        private readonly ILogger<GestorEquiposGuiaController> _logger;
        private readonly EquiposGuiaDAO _equiposGuiaDAO;
        private readonly ProfesorDAO _profesorDAO;

        public GestorEquiposGuiaController(ILogger<GestorEquiposGuiaController> logger)
        {
            _logger = logger;
            _equiposGuiaDAO = new EquiposGuiaDAO();
            _profesorDAO = new ProfesorDAO();
        }

        public ActionResult GestorEquiposGuia()
        {
            var equipos = _equiposGuiaDAO.RecuperarEquiposGuia();
            ViewData["lista_ProfesoresDisponibles"] = ObtenerProfesoresDisponibles();
            return View("~/Views/Pages/GestorEquiposGuia.cshtml", equipos);
        }


        private List<Profesor> ObtenerProfesoresDisponibles()
        {
            return _profesorDAO.recuperarProfesores();
        }

        #region VISTA DE INTEGRANTES

        [HttpGet]
        public IActionResult GetIntegrantes(int generacion)
        {
            var equipo = _equiposGuiaDAO.RecuperarEquiposGuia().FirstOrDefault(e => e.Generacion == generacion);
            if (equipo != null)
            {
                return Json(new
                {
                    success = true,
                    data = equipo.Integrantes.Select(integrante => new
                    {
                        Nombre1 = integrante.Nombre1,
                        Apellido1 = integrante.Apellido1
                    })
                });
            }
            return Json(new { success = false, message = "Equipo no encontrado." });
        }

        #endregion VISTA DE INTEGRANTES

        #region EDITAR EQUIPO


        /// <summary>
        /// Se llama desde el Front End para que retorne los detalles del Equipo que se seleccionó para editar
        /// </summary>
        /// <param name="generacion"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetEquipoDetails(int generacion)
        {
            var equipo = _equiposGuiaDAO.RecuperarEquiposGuia().FirstOrDefault(e => e.Generacion == generacion);
            if (equipo != null)
            {
                return Json(new
                {
                    success = true,
                    integrantes = equipo.Integrantes.Select(i => new
                    {
                        Codigo = i.Codigo,
                        Nombre1 = i.Nombre1,
                        Apellido1 = i.Apellido1
                    }),
                    coordinador = new
                    {
                        Codigo = equipo.Coordinador?.Codigo,
                        Nombre1 = equipo.Coordinador?.Nombre1,
                        Apellido1 = equipo.Coordinador?.Apellido1
                    }
                });
            }
            return Json(new { success = false, message = "Equipo no encontrado." });
        }


        /// <summary>
        /// Se llama desde el View para que se agregue un nuevo integrante a un equipo seleccionado para editar
        /// </summary>
        /// <param name="generacion"></param>
        /// <param name="codigoProfesor"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AgregarIntegrante_EditarEquipo(int generacion, string codigoProfesor)
        {
            var profesor = _profesorDAO.recuperarProfesores().FirstOrDefault(p => p.Codigo == codigoProfesor);
            if (profesor != null)
            {
                var equipo = _equiposGuiaDAO.RecuperarEquiposGuia().FirstOrDefault(e => e.Generacion == generacion);
                if (equipo != null && (equipo.Coordinador?.Codigo == codigoProfesor || equipo.Integrantes.Any(i => i.Codigo == codigoProfesor)))
                {
                    return Json(new { success = false, message = "El profesor ya es integrante o coordinador del equipo." });
                }

                bool success = _equiposGuiaDAO.AgregarIntegrante(generacion, profesor);
                if (success)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Profesor añadido exitosamente.",
                        data = new
                        {
                            Codigo = profesor.Codigo,
                            Nombre1 = profesor.Nombre1,
                            Apellido1 = profesor.Apellido1
                        }
                    });
                }
                return Json(new { success = false, message = "Error al agregar el integrante." });
            }
            return Json(new { success = false, message = "Profesor no encontrado." });
        }



        /// <summary>
        /// Se llama desde el View para eliminar un integrante de un equipo seleccionado para editar
        /// </summary>
        /// <param name="generacion"></param>
        /// <param name="codigoProfesor"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RemoverIntegrante_EditarEquipo(int generacion, string codigoProfesor)
        {
            var equipo = _equiposGuiaDAO.RecuperarEquiposGuia().FirstOrDefault(e => e.Generacion == generacion);
            if (equipo != null)
            {
                var profesor = equipo.Integrantes.FirstOrDefault(p => p.Codigo == codigoProfesor);
                if (profesor != null)
                {
                    bool success = _equiposGuiaDAO.EliminarIntegrante(generacion, codigoProfesor);
                    if (success)
                    {
                        equipo.Integrantes.Remove(profesor);
                        return Json(new { success = true, message = "Integrante removido exitosamente." });
                    }
                    return Json(new { success = false, message = "Error al eliminar el integrante." });
                }
                return Json(new { success = false, message = "Integrante no encontrado." });
            }
            return Json(new { success = false, message = "Equipo no encontrado." });
        }


        /// <summary>
        /// Se llama desde el View para agregar un coodrinador a un equipo seleccionado para editar
        /// </summary>
        /// <param name="generacion"></param>
        /// <param name="codigoProfesor"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AgregarCoodinador_EditarEquipo(int generacion, string codigoProfesor)
        {
            var profesor = _profesorDAO.recuperarProfesores().FirstOrDefault(p => p.Codigo == codigoProfesor);
            if (profesor != null)
            {
                var equipo = _equiposGuiaDAO.RecuperarEquiposGuia().FirstOrDefault(e => e.Generacion == generacion);
                if (equipo != null && (equipo.Coordinador?.Codigo == codigoProfesor || equipo.Integrantes.Any(i => i.Codigo == codigoProfesor)))
                {
                    return Json(new { success = false, message = "El profesor ya es integrante o coordinador del equipo." });
                }

                bool success = _equiposGuiaDAO.AsignarCoordinador(generacion, profesor);
                if (success)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Coordinador asignado correctamente.",
                        data = new
                        {
                            Codigo = profesor.Codigo,
                            Nombre1 = profesor.Nombre1,
                            Apellido1 = profesor.Apellido1
                        }
                    });
                }
                return Json(new { success = false, message = "Error al asignar el coordinador." });
            }
            return Json(new { success = false, message = "Profesor no encontrado." });
        }


        #endregion EDITAR EQUIPO

        #region CREAR NUEVO EQUIPO

        /// <summary>
        /// Recibe todos los datos necesario para crear un nuevo equipo
        /// </summary>
        /// <param name="equipo"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CrearNuevoEquipo([FromBody] EquipoCreacionDto equipo)
        {
            if (equipo.CodigoCoordinador == null || equipo.CodigosProfesores.Count == 0 || equipo.Generacion == 0)
            {
                return Json(new { success = false, message = "Datos no fueron recibidos correctamente." });
            }

            var coordinador = _profesorDAO.recuperarProfesores().FirstOrDefault(p => p.Codigo == equipo.CodigoCoordinador);
            var profesores = _profesorDAO.recuperarProfesores().Where(p => equipo.CodigosProfesores.Contains(p.Codigo)).ToList();

            if (coordinador == null || profesores.Count == 0)
            {
                return Json(new { success = false, message = "Coordinador o profesores no encontrados." });
            }

            try
            {
                EquipoGuia nuevoEquipo = new EquipoGuia
                {
                    Coordinador = coordinador,
                    Integrantes = profesores,
                    Generacion = equipo.Generacion
                };

                bool success = _equiposGuiaDAO.RegistrarEquipoGuia(nuevoEquipo);
                if (success)
                {
                    return Json(new { success = true, message = "Equipo creado correctamente" });
                }
                return Json(new { success = false, message = "Error al crear el equipo." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo equipo");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        #region DTO PARA RECIBIR DATOS DE CREACIÓN DE NUEVO EQUIPO

        public class EquipoCreacionDto
        {
            public string CodigoCoordinador { get; set; }
            public List<string> CodigosProfesores { get; set; }
            public int Generacion { get; set; }
        }

        #endregion DTO PARA RECIBIR DATOS DE CREACIÓN DE NUEVO EQUIPO

        #endregion CREAR NUEVO EQUIPO
    }
}
