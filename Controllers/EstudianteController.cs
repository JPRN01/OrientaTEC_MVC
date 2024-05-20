using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Diagnostics;
using OfficeOpenXml;
using System.IO;


namespace OrientaTEC_MVC.Controllers
{
	public class EstudianteController : Controller
	{
		public EstudianteController()
		{
			//
			// TODO: Add constructor logic here
			//
		}
        public void CargarInformacionEstudiantes(string archivoExcel)
        {
            using (var stream = System.IO.File.OpenRead(archivoExcel))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var totalRows = worksheet.Dimension.Rows;
                    for (int row = 2; row <= totalRows; row++)
                    {
                        var estudiante = new Estudiante
                        {
                            Carne = int.Parse(worksheet.Cells[row, 1].Value.ToString().Trim()),
                            Apellido1 = worksheet.Cells[row, 2].Value.ToString().Split(' ')[0].Trim(),
                            Apellido2 = worksheet.Cells[row, 2].Value.ToString().Split(' ')[1].Trim(),
                            Nombre1 = worksheet.Cells[row, 2].Value.ToString().Split(' ')[2].Trim(),
                            Nombre2 = worksheet.Cells[row, 2].Value.ToString().Split(' ').Length > 3 ? worksheet.Cells[row, 2].Value.ToString().Split(' ')[3].Trim() : null,
                            Correo = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            TelCelular = int.Parse(worksheet.Cells[row, 4].Value.ToString().Trim()),
                            CentroAcademico = "Centro" //Hay que cambiarlo para recuperar el centro segun el asistente que lo sube 
                        };

                        bool resultado = EstudianteDAO_Singleton.Instance.AgregarEstudiante(estudiante);
                        if (!resultado)
                        {
                            Console.WriteLine($"Error al insertar el estudiante con carné: {estudiante.Carne}");
                        }
                    }
                }
            }
        }

    }
}
