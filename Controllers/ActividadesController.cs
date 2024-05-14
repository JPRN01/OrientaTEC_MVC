using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrientaTEC_MVC.ViewModels;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ActividadesController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ActividadesController> _logger;

    public ActividadesController(IConfiguration configuration, ILogger<ActividadesController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    [HttpGet]
    public IActionResult PlaneacionActividades(int? generacionId = null)
    {
        var equipoGuiaViewModel = new EquipoGuiaViewModel
        {
            EquiposGuia = new List<SelectListItem>()
        };

        var actividadesViewModel = new ActividadesViewModel
        {
            Actividades = new List<Actividad>()
        };

        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        string queryEquiposGuia = "SELECT generacion, GENERACION as Value FROM Equipo_Guia";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand commandEquiposGuia = new SqlCommand(queryEquiposGuia, connection);
            connection.Open();

            SqlDataReader reader = commandEquiposGuia.ExecuteReader();
            while (reader.Read())
            {
                equipoGuiaViewModel.EquiposGuia.Add(new SelectListItem
                {
                    Text = $"Generación {reader["generacion"]}",
                    Value = reader["Value"].ToString()
                });
            }
            reader.Close();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                string queryActividades = @"
                SELECT a.ID_ACTIVIDAD, a.nombre, a.descripcion, ea.estado AS EstadoNombre
                FROM dbo.Actividad a
                JOIN dbo.Plan_Trabajo p ON a.ID_PLAN = p.ID_PLAN
                JOIN dbo.Estado_Registrado er ON a.ID_ESTADO_REGISTRADO = er.ID_ESTADO_REGISTRADO
                JOIN dbo.Estado_Actividad ea ON er.ID_ESTADO_ACTIVIDAD = ea.ID_ESTADO_ACTIVIDAD
                WHERE Generacion IS NULL OR p.GENERACION = @Generacion";

                SqlCommand commandActividades = new SqlCommand(queryActividades, connection);
                commandActividades.Parameters.AddWithValue("@Generacion", (object)generacionId ?? DBNull.Value);

                reader = commandActividades.ExecuteReader();
                while (reader.Read())
                {
                    var actividad = new Actividad
                    {
                        IdActividad = (int)reader["ID_ACTIVIDAD"],
                        Nombre = reader["nombre"].ToString(),
                        Descripcion = reader["descripcion"].ToString(),
                        Estado = new EstadoRegistrado()
                    };

                    if (reader["EstadoNombre"] != DBNull.Value)
                    {
                        actividad.Estado.Estado = (EstadoActividad)Enum.Parse(typeof(EstadoActividad), reader["EstadoNombre"].ToString());
                    }

                    actividadesViewModel.Actividades.Add(actividad);
                }
                reader.Close();

                return Json(actividadesViewModel.Actividades.Select(a => new {
                    a.IdActividad,
                   
                    a.Nombre,
                    a.Descripcion,
                    Estado = a.Estado.Estado.ToString()
                }));
            }
        }

        ViewBag.ActividadesViewModel = actividadesViewModel;
        return View("~/Views/Pages/PlaneacionActividades.cshtml", equipoGuiaViewModel);
    }

}
