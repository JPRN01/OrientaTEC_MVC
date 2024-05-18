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


   [HttpPost]
    public async Task<IActionResult> CrearActividad(string nombre, int generacionId, string tipo, string modalidad, string estado, string enlace, int diasPreviosParaAnunciar)
    {
        try
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");




            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Validar si la generación 
                string checkGeneracion = "SELECT COUNT(1) FROM Equipo_Guia WHERE GENERACION = @Generacion";
                SqlCommand checkCommand = new SqlCommand(checkGeneracion, connection);
                checkCommand.Parameters.AddWithValue("@Generacion", generacionId);
                int exists = (int)checkCommand.ExecuteScalar();

                if (exists == 0)
                {
                    _logger.LogError("La generación especificada no existe.");
                    return View("Error");
                }

                // Insertar en Plan_Trabajo
                string planInsert = "INSERT INTO Plan_Trabajo (GENERACION) OUTPUT INSERTED.ID_PLAN VALUES (@Generacion)";
                SqlCommand planCommand = new SqlCommand(planInsert, connection);
                planCommand.Parameters.AddWithValue("@Generacion", generacionId);
                int idPlan = (int)planCommand.ExecuteScalar();

                // Insertar en Actividad
                string actividadInsert = @"
                INSERT INTO Actividad (Nombre, ID_PLAN, ID_TIPO_ACTIVIDAD, Descripcion, Semana, Fecha_Exacta, Dias_Previos_Para_Anunciar, Dias_Para_Recordar, Es_Virtual, Reunion_URL, Afiche_URL, ID_ESTADO_REGISTRADO)
                VALUES (@Nombre, @IdPlan, @Tipo, 'Descripción default', 1, GETDATE(), @DiasPreviosParaAnunciar, 0, @Modalidad, @Enlace, '', @Estado)";
                SqlCommand actividadCommand = new SqlCommand(actividadInsert, connection);
                actividadCommand.Parameters.AddWithValue("@Nombre", nombre);
                actividadCommand.Parameters.AddWithValue("@IdPlan", idPlan);
                actividadCommand.Parameters.AddWithValue("@Tipo", tipo);
                actividadCommand.Parameters.AddWithValue("@Modalidad", modalidad == "Presencial" ? 0 : 1); // Asume que '0' es Presencial y '1' es Remoto
                actividadCommand.Parameters.AddWithValue("@Estado", estado); // Asegúrate de que el estado es un ID numérico si es foráneo
                actividadCommand.Parameters.AddWithValue("@Enlace", enlace);
                actividadCommand.Parameters.AddWithValue("@DiasPreviosParaAnunciar", diasPreviosParaAnunciar);

                await actividadCommand.ExecuteNonQueryAsync();
            }
            return RedirectToAction("PlaneacionActividades");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear actividad: {ex.Message}");
            return View("Error");
        }
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

        // Asegúrate de que los modelos no son null antes de pasar a la vista
        ViewBag.ActividadesViewModel = actividadesViewModel ?? new ActividadesViewModel();
        return View("~/Views/Pages/PlaneacionActividades.cshtml", equipoGuiaViewModel ?? new EquipoGuiaViewModel());

    }

}
