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
        private readonly EstudianteDAO _estudianteDAO;
        private readonly CentroAcademicoDAO _centroAcademicoDAO;

        public GestorEstudiantesController(ILogger<GestorEstudiantesController> logger)
        {
            _logger = logger;
            _estudianteDAO = new EstudianteDAO();
            _centroAcademicoDAO = new CentroAcademicoDAO();
        }

        public IActionResult GestorEstudiantes()
        {
            var centrosAcademicos = _centroAcademicoDAO.RecuperarCentrosAcademicos();
            ViewData["CentrosAcademicos"] = centrosAcademicos;
            var estudiantes = _estudianteDAO.RecuperarEstudiantes();
            return View("~/Views/Pages/GestorEstudiantes.cshtml", estudiantes);
        }

        public IActionResult GetEstudiante(int carne)
        {
            var estudiante = _estudianteDAO.RecuperarEstudiantes().FirstOrDefault(e => e.Carne == carne);
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
                        estudiante.Carne,
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
            if (_estudianteDAO.ModificarEstudiante(estudiante))
            {
                return Json(new { success = true, message = "Estudiante actualizado correctamente." });
            }
            return Json(new { success = false, message = "Error al actualizar el estudiante." });
        }

        [HttpPost]
        public IActionResult EliminarEstudiante([FromBody] Estudiante estudiante)
        {
            if (_estudianteDAO.EliminarEstudiante(estudiante.Carne))
            {
                return Json(new { success = true, message = "Estudiante eliminado correctamente." });
            }
            return Json(new { success = false, message = "Error al eliminar el estudiante." });
        }

        [HttpPost]
        public async Task<IActionResult> CargarEstudiantes(IFormFile fileUpload, string centroAcademico)
        {
            if (fileUpload == null || fileUpload.Length == 0)
            {
                return Json(new { success = false, message = "Por favor seleccione un archivo válido." });
            }

            var estudiantesNuevos = new List<Estudiante>();
            var carneDuplicados = new HashSet<int>();

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
                        var carne = int.Parse(worksheet.Cells[row, 1].Text);

                        if (_estudianteDAO.RecuperarEstudiantes().Any(e => e.Carne == carne) || carneDuplicados.Contains(carne))
                        {
                            return Json(new { success = false, message = $"Carne duplicado encontrado: {carne}. La carga ha sido abortada." });
                        }

                        var estudiante = new Estudiante
                        {
                            Carne = carne,
                            Nombre1 = worksheet.Cells[row, 2].Text,
                            Nombre2 = worksheet.Cells[row, 3].Text,
                            Apellido1 = worksheet.Cells[row, 4].Text,
                            Apellido2 = worksheet.Cells[row, 5].Text,
                            Correo = worksheet.Cells[row, 6].Text,
                            TelCelular = int.Parse(worksheet.Cells[row, 7].Text),
                            CentroAcademico = centroAcademico
                        };
                        estudiantesNuevos.Add(estudiante);
                        carneDuplicados.Add(carne);
                    }
                }
            }

            foreach (var estudiante in estudiantesNuevos)
            {
                _estudianteDAO.RegistrarEstudiante(estudiante);
            }

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
                worksheet.Cells[1, 2].Value = "Carne";
                worksheet.Cells[1, 3].Value = "Nombre1";
                worksheet.Cells[1, 4].Value = "Nombre2";
                worksheet.Cells[1, 5].Value = "Apellido1";
                worksheet.Cells[1, 6].Value = "Apellido2";
                worksheet.Cells[1, 7].Value = "Correo";
                worksheet.Cells[1, 8].Value = "TelCelular";

                var estudiantes = _estudianteDAO.RecuperarEstudiantes();
                for (int i = 0; i < estudiantes.Count; i++)
                {
                    var estudiante = estudiantes[i];
                    worksheet.Cells[i + 2, 1].Value = estudiante.CentroAcademico;
                    worksheet.Cells[i + 2, 2].Value = estudiante.Carne;
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
    }
}
