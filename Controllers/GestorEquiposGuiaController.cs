using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace OrientaTEC_MVC.Controllers
{
    public class GestorEquiposGuiaController : Controller
    {
        private readonly ILogger<GestorEquiposGuiaController> _logger;
        private List<EquipoGuia> equipos; // Simula conexión a DAO // REEMPLAZAR CON CONEXIONES REALES
        private static List<Profesor> _profesores; // Simula conexión a DAO // REEMPLAZAR CON CONEXIONES REALES

        public GestorEquiposGuiaController(ILogger<GestorEquiposGuiaController> logger)
        {
            _logger = logger;

            if (equipos == null)
            {
                equipos = InicializarEquiposGuia();  

            }
        }


        public ActionResult GestorEquiposGuia()
        {

            ViewData["lista_ProfesoresDisponibles"] = ObtenerProfesoresDisponibles();

            return View("~/Views/Pages/GestorEquiposGuia.cshtml", equipos);
        }

        
        private List<Profesor> ObtenerProfesoresDisponibles()
        {

            //AGREGAR CONEXIÓN A BASE DE DATOS / DAO ACÁ

            return InicializarProfesores();
        }





        // REEMPLAZAR CON CONEXIÓN A BASE DE DATOS / DAO / MODELO
        private List<EquipoGuia> InicializarEquiposGuia()
        {
            var equiposGuia = new List<EquipoGuia>();
            for (int i = 1; i <= 5; i++) // Create 5 example teams
            {
                var coordinador = new Profesor
                {
                    Codigo = (i % 2 == 0 ? "CA" : "SJ") + "001" + i.ToString(),
                    Nombre1 = "Coordinador" + i,  // Make sure to adjust these properties to match your Profesor class
                    Apellido1 = "Apellido" + i,
                    Correo = "coordinador" + i + "@example.edu",
                    TelCelular = 555000 + i
                };

                var integrantes = new List<Profesor>();
                for (int j = 1; j <= 3; j++) // Each team has 3 members
                {
                    integrantes.Add(new Profesor
                    {
                        Codigo = (i % 2 == 0 ? "CA" : "SJ") + "000" + j.ToString(),
                        Nombre1 = "Profesor" + (i * 3 + j),
                        Apellido1 = "Apellido" + (i * 3 + j),
                        Correo = "profesor" + (i * 3 + j) + "@example.edu",
                        TelCelular = 555100 + (i * 3 + j)
                    });
                }

                equiposGuia.Add(new EquipoGuia
                {
                    Generacion = 2019 + i,
                    Coordinador = coordinador,
                    Integrantes = integrantes
                });
            }
            return equiposGuia;
        }

        // REEMPLAZAR CON CONEXIÓN A BASE DE DATOS / DAO / MODELO


        private List<Profesor> InicializarProfesores()
        {
            _profesores = new List<Profesor>();
            for (int i = 1; i <= 20; i++)
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
            }
            return _profesores;
        }




        #region VISTA DE INTEGRANTES

        [HttpGet]
        public IActionResult GetIntegrantes(int generacion)
        {
            var equipo = equipos.FirstOrDefault(e => e.Generacion == generacion);
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
        /// Se encarga de cargar en el view los valores de un equipo para su edición al abrir el modal de Editar
        /// </summary>
        /// <param name="generacion"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetEquipoDetails(int generacion)
        {
            var equipo = equipos.FirstOrDefault(e => e.Generacion == generacion);
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
                        Codigo = equipo.Coordinador.Codigo,
                        Nombre1 = equipo.Coordinador.Nombre1,
                        Apellido1 = equipo.Coordinador.Apellido1
                    }
                });
            }
            return Json(new { success = false, message = "Equipo no encontrado." });
        }


        /// <summary>
        /// Se agregan profesores integrantes al equipo 
        /// </summary>
        /// <param name="generacion"></param>
        /// <param name="codigoProfesor"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AgregarIntegrante_EditarEquipo(int generacion, string codigoProfesor)
        {
            EquipoGuia? equipo = equipos.FirstOrDefault(e => e.Generacion == generacion);
            if (equipo != null)
            {
                Profesor? profesor = _profesores.FirstOrDefault(p => p.Codigo == codigoProfesor);
                if (profesor != null)
                {
                    if (!equipo.Integrantes.Any(p => p.Codigo == codigoProfesor))
                    {
                        equipo.Integrantes.Add(profesor);
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
                    else
                    {
                        return Json(new { success = false, message = "Profesor seleccionado ya es miembro del Equipo Guía." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Profesor no encontrado." });
                }
            }
            return Json(new { success = false, message = "Equipo no encontrado." });
        }



        /// <summary>
        /// Hace la función de delete de integrantes
        /// </summary>
        /// <param name="generacion"></param>
        /// <param name="codigoProfesor"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RemoverIntegrante_EditarEquipo(int generacion, string codigoProfesor)
        {
            EquipoGuia? equipo = equipos.FirstOrDefault(e => e.Generacion == generacion);
            if (equipo != null)
            {

                Profesor? profesor = equipo.Integrantes.FirstOrDefault(p => p.Codigo == codigoProfesor);
                if (profesor != null)
                {
                    equipo.Integrantes.Remove(profesor);
                    return Json(new { success = true, message = "Integrante removido exitosamente." });
                }
                else
                {
                    return Json(new { success = false, message = "Integrante no encontrado." });
                }
            }
            return Json(new { success = false, message = "Equipo no encontrado." });
        }

        /// <summary>
        /// Agrega un coordinador al equipo actual. Reemplaza el coordinador que había
        /// </summary>
        /// <param name="generacion"></param>
        /// <param name="codigoProfesor"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AgregarCoodinador_EditarEquipo(int generacion, string codigoProfesor)
        {
            EquipoGuia? equipo = equipos.FirstOrDefault(e => e.Generacion == generacion);
            if (equipo != null)
            {
                Profesor? profesor = _profesores.FirstOrDefault(p => p.Codigo == codigoProfesor);
                if (profesor != null)
                {
                    equipo.Integrantes.Add(profesor);
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
                else
                {
                    return Json(new { success = false, message = "Profesor no encontrado." });
                }
            }
            return Json(new { success = false, message = "Equipo no encontrado." });
        }

        #endregion EDITAR EQUIPO



        #region CREAR NUEVO EQUIPO

        /// <summary>
        /// Codigo necesario para la creación de nuevos equipos
        /// </summary>
        /// <param name="codigoCoordinador"></param>
        /// <param name="codigosProfesores"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CrearNuevoEquipo([FromBody] EquipoCreacionDto equipo)
        {
            if (equipo.CodigoCoordinador == null || equipo.CodigosProfesores.Count == 0 || equipo.Generacion == 0)
            {
                return Json(new { success = false, message = "Datos no fueron recibidos correctamente." });
            }

            Profesor? coordinador = _profesores.FirstOrDefault(p => p.Codigo == equipo.CodigoCoordinador);
            List<Profesor> profesores = _profesores.Where(p => equipo.CodigosProfesores.Contains(p.Codigo)).ToList();

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

                equipos.Add(nuevoEquipo);
                return Json(new { success = true, message = "Equipo creado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo equipo");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }


        #region DTO PARA RECIBIR DATOS DE CREACIÓN DE NUEVO EQUIPO
        /// <summary>
        /// Clase interna que funciona como DTO para la creación de equipos
        /// </summary>
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
