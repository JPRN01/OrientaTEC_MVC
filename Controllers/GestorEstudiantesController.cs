using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrientaTEC_MVC.Controllers
{
    public class GestorEstudiantesController : Controller
    {
        private readonly ILogger<GestorEstudiantesController> _logger;
        private static List<Estudiante> _estudiantes;
        private static List<CentroAcademico> _centrosAcademicos;

        public GestorEstudiantesController(ILogger<GestorEstudiantesController> logger)
        {
            _logger = logger;
            if (_estudiantes == null)
            {
                _estudiantes = InicializarEstudiantes();
            }
            if (_centrosAcademicos == null)
            {
                _centrosAcademicos = InicializarCentrosAcademicos();
            }
        }

        public IActionResult GestorEstudiantes()
        {
            ViewData["CentrosAcademicos"] = _centrosAcademicos;
            return View("~/Views/Pages/GestorEstudiantes.cshtml", _estudiantes);
        }

        public IActionResult GetEstudiante(int carnet)
        {
            var estudiante = _estudiantes.FirstOrDefault(e => e.Carnet == carnet);
            if (estudiante != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        estudiante.CentroAcademico,
                        estudiante.Nombre1,
                        estudiante.Nombre2,
                        estudiante.Apellido1,
                        estudiante.Apellido2,
                        estudiante.Carnet,
                        estudiante.TelCelular,
                        estudiante.Correo
                    }
                });
            }
            return Json(new { success = false, message = "Estudiante no encontrado." });
        }

        [HttpPost]
        public IActionResult GuardarEstudiante([FromBody] Estudiante estudiante)
        {
            var estudianteExistente = _estudiantes.FirstOrDefault(e => e.Carnet == estudiante.Carnet);
            if (estudianteExistente != null)
            {
                estudianteExistente.CentroAcademico = estudiante.CentroAcademico;
                estudianteExistente.Nombre1 = estudiante.Nombre1;
                estudianteExistente.Nombre2 = estudiante.Nombre2;
                estudianteExistente.Apellido1 = estudiante.Apellido1;
                estudianteExistente.Apellido2 = estudiante.Apellido2;
                estudianteExistente.TelCelular = estudiante.TelCelular;
                return Json(new { success = true, message = "Estudiante actualizado correctamente." });
            }
            return Json(new { success = false, message = "Estudiante no encontrado." });
        }

        [HttpPost]
        public IActionResult EliminarEstudiante([FromBody] Estudiante estudiante)
        {
            var estudianteExistente = _estudiantes.FirstOrDefault(e => e.Carnet == estudiante.Carnet);
            if (estudianteExistente != null)
            {
                _estudiantes.Remove(estudianteExistente);
                return Json(new { success = true, message = "Estudiante eliminado correctamente." });
            }
            return Json(new { success = false, message = "Estudiante no encontrado." });
        }


        [HttpPost]
        public async Task<IActionResult> CargarEstudiantes(IFormFile fileUpload, string centroAcademico)
        {
            if (fileUpload == null || fileUpload.Length == 0)
            {
                return Json(new { success = false, message = "Por favor seleccione un archivo válido." });
            }

            var estudiantesNuevos = new List<Estudiante>();
            var carnetDuplicados = new HashSet<int>();

            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await fileUpload.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var carnet = int.Parse(worksheet.Cells[row, 1].Text);

                        if (_estudiantes.Any(e => e.Carnet == carnet) || carnetDuplicados.Contains(carnet))
                        {
                            return Json(new { success = false, message = $"Carnet duplicado encontrado: {carnet}. La carga ha sido abortada." });
                        }

                        var estudiante = new Estudiante
                        {
                            Carnet = carnet,
                            Nombre1 = worksheet.Cells[row, 2].Text,
                            Nombre2 = worksheet.Cells[row, 3].Text,
                            Apellido1 = worksheet.Cells[row, 4].Text,
                            Apellido2 = worksheet.Cells[row, 5].Text,
                            Correo = worksheet.Cells[row, 6].Text,
                            TelCelular = int.Parse(worksheet.Cells[row, 7].Text),
                            CentroAcademico = centroAcademico
                        };
                        estudiantesNuevos.Add(estudiante);
                        carnetDuplicados.Add(carnet);
                    }
                }
            }

            _estudiantes.AddRange(estudiantesNuevos);

            return Json(new { success = true, message = $"{estudiantesNuevos.Count} estudiantes cargados exitosamente." });
        }



        [HttpGet]
        public IActionResult DescargarEstudiantes()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Estudiantes");
                worksheet.Cells[1, 1].Value = "CentroAcademico";
                worksheet.Cells[1, 2].Value = "Carnet";
                worksheet.Cells[1, 3].Value = "Nombre1";
                worksheet.Cells[1, 4].Value = "Nombre2";
                worksheet.Cells[1, 5].Value = "Apellido1";
                worksheet.Cells[1, 6].Value = "Apellido2";
                worksheet.Cells[1, 7].Value = "Correo";
                worksheet.Cells[1, 8].Value = "TelCelular";

                for (int i = 0; i < _estudiantes.Count; i++)
                {
                    var estudiante = _estudiantes[i];
                    worksheet.Cells[i + 2, 1].Value = estudiante.CentroAcademico;
                    worksheet.Cells[i + 2, 2].Value = estudiante.Carnet;
                    worksheet.Cells[i + 2, 3].Value = estudiante.Nombre1;
                    worksheet.Cells[i + 2, 4].Value = estudiante.Nombre2;
                    worksheet.Cells[i + 2, 5].Value = estudiante.Apellido1;
                    worksheet.Cells[i + 2, 6].Value = estudiante.Apellido2;
                    worksheet.Cells[i + 2, 7].Value = estudiante.Correo;
                    worksheet.Cells[i + 2, 8].Value = estudiante.TelCelular;
                }

                package.Save();
            }

            stream.Position = 0;
            var content = stream.ToArray();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "Estudiantes.xlsx";

            return File(content, contentType, fileName);
        }



        private List<Estudiante> InicializarEstudiantes()
        {
            return new List<Estudiante>
            {
                new Estudiante { Carnet = 1997032412, Nombre1 = "Marie", Nombre2 = "S.", Apellido1 = "Curie", Apellido2 = "", Correo = "curie.m@estudiantec.cr", TelCelular = 88852408, CentroAcademico = "SJ" },
                new Estudiante { Carnet = 1815121010, Nombre1 = "Ada", Nombre2 = "", Apellido1 = "Lovelace", Apellido2 = "", Correo = "lovelace.a@estudiantec.cr", TelCelular = 87654321, CentroAcademico = "CA" },
                new Estudiante { Carnet = 1912062312, Nombre1 = "Alan", Nombre2 = "M.", Apellido1 = "Turing", Apellido2 = "", Correo = "turing.a@estudiantec.cr", TelCelular = 85678901, CentroAcademico = "SJ" },
                new Estudiante { Carnet = 1934110902, Nombre1 = "Carl", Nombre2 = "E.", Apellido1 = "Sagan", Apellido2 = "", Correo = "sagan.c@estudiantec.cr", TelCelular = 84567890, CentroAcademico = "SJ" },
                new Estudiante { Carnet = 1927090415, Nombre1 = "John", Nombre2 = "", Apellido1 = "McCarthy", Apellido2 = "", Correo = "mccarthy.j@estudiantec.cr", TelCelular = 87901234, CentroAcademico = "CA" },
                new Estudiante { Carnet = 1903122810, Nombre1 = "John", Nombre2 = "", Apellido1 = "von Neumann", Apellido2 = "", Correo = "vonneumann.j@estudiantec.cr", TelCelular = 85678901, CentroAcademico = "SJ" }
            };
        }

        private List<CentroAcademico> InicializarCentrosAcademicos()
        {
            return new List<CentroAcademico>
            {
                new CentroAcademico { Iniciales = "SJ", Nombre = "San José", EsSedeCentral = true },
                new CentroAcademico { Iniciales = "CA", Nombre = "Cartago", EsSedeCentral = false },
                new CentroAcademico { Iniciales = "AL", Nombre = "Alajuela", EsSedeCentral = false },
                new CentroAcademico { Iniciales = "LI", Nombre = "Limón", EsSedeCentral = false },
                new CentroAcademico { Iniciales = "HE", Nombre = "Heredia", EsSedeCentral = false }
            };
        }
    }
}
