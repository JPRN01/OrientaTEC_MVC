using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrientaTEC_MVC.ViewModels;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

public class ActividadesController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ActividadesController> _logger;

    public ActividadesController(IConfiguration configuration, ILogger<ActividadesController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    public async Task<IActionResult> ObtenerProfesores(int generacionId)
    {
        List<ProfesorViewModel> profesores = new List<ProfesorViewModel>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        string query = @"
        SELECT DISTINCT p.NUMERO, p.nombre1, p.nombre2, p.apellido1, p.apellido2
        FROM dbo.Profesor p
        JOIN dbo.Profesor_X_Equipo_Guia pxeg ON p.NUMERO = pxeg.NUMERO
        WHERE pxeg.GENERACION = @Generacion AND pxeg.esta_activo = 1";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Generacion", generacionId);
            try
            {
                connection.Open();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        string nombreCompleto = $"{reader["nombre1"]} {reader["nombre2"]} {reader["apellido1"]} {reader["apellido2"]}".Replace("  ", " ").Trim();
                        profesores.Add(new ProfesorViewModel
                        {
                            Numero = (int)reader["NUMERO"],
                            NombreCompleto = nombreCompleto
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ObtenerProfesores: {ex.Message}");
                return StatusCode(500, "Error al cargar profesores");
            }
        }
        return Json(profesores);
    }





    [HttpPost]
    public async Task<IActionResult> CrearActividad(string nombre, int generacionId, string tipoId, bool esVirtual, string estadoId, string enlace, int diasPreviosParaAnunciar, int diasPreviosParaRecordar, int semana, string descripcion, IFormFile afiche, int[] profesoresIds)
    {
        try
        {
            _logger.LogInformation($"Profesores IDs recibidos: {string.Join(", ", profesoresIds)}");

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string afichePath = null;

                if (afiche != null)
                {
                    string fileName = Path.GetFileName(afiche.FileName);
                    afichePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                    using (var stream = new FileStream(afichePath, FileMode.Create))
                    {
                        await afiche.CopyToAsync(stream);
                    }
                }

                // Inserta un nuevo plan de trabajo y obtén el ID
                string planInsert = "INSERT INTO Plan_Trabajo (GENERACION) OUTPUT INSERTED.ID_PLAN VALUES (@Generacion)";
                SqlCommand planCommand = new SqlCommand(planInsert, connection);
                planCommand.Parameters.AddWithValue("@Generacion", generacionId);
                int idPlan = (int)planCommand.ExecuteScalar();

                // Inserta la nueva actividad y obtén el ID
                string actividadInsert = "INSERT INTO Actividad (nombre, ID_PLAN, ID_TIPO_ACTIVIDAD, descripcion, semana, fecha_exacta, dias_previos_para_anunciar, dias_para_recordar, es_virtual, reunion_url, afiche_url, ID_ESTADO_REGISTRADO) OUTPUT INSERTED.ID_ACTIVIDAD VALUES (@Nombre, @IdPlan, @TipoId, @Descripcion, @Semana, GETDATE(), @DiasPreviosParaAnunciar, @DiasParaRecordar, @EsVirtual, @Enlace, @AficheURL, @EstadoId)";
                SqlCommand actividadCommand = new SqlCommand(actividadInsert, connection);
                actividadCommand.Parameters.AddWithValue("@Nombre", nombre);
                actividadCommand.Parameters.AddWithValue("@IdPlan", idPlan);
                actividadCommand.Parameters.AddWithValue("@TipoId", tipoId);
                actividadCommand.Parameters.AddWithValue("@Descripcion", descripcion);
                actividadCommand.Parameters.AddWithValue("@Semana", semana);
                actividadCommand.Parameters.AddWithValue("@DiasPreviosParaAnunciar", diasPreviosParaAnunciar);
                actividadCommand.Parameters.AddWithValue("@DiasParaRecordar", diasPreviosParaRecordar);
                actividadCommand.Parameters.AddWithValue("@EsVirtual", esVirtual ? 1 : 0); // Asegúrate de que es_virtual sea un tipo de dato compatible
                actividadCommand.Parameters.AddWithValue("@Enlace", enlace);
                actividadCommand.Parameters.AddWithValue("@AficheURL", afichePath);
                actividadCommand.Parameters.AddWithValue("@EstadoId", estadoId);

                int idActividad = (int)actividadCommand.ExecuteScalar();






                foreach (int profesorId in profesoresIds)
                {
                    string asignacionInsert = "INSERT INTO Profesor_X_Equipo_Guia_X_Actividad (ID_ACTIVIDAD, NUMERO, GENERACION) VALUES (@IdActividad, @ProfesorId, @Generacion)";
                    SqlCommand asignacionCommand = new SqlCommand(asignacionInsert, connection);
                    asignacionCommand.Parameters.AddWithValue("@IdActividad", idActividad);
                    asignacionCommand.Parameters.AddWithValue("@ProfesorId", profesorId);
                    asignacionCommand.Parameters.AddWithValue("@Generacion", generacionId); 
                    await asignacionCommand.ExecuteNonQueryAsync();
                }


            }
            return RedirectToAction("PlaneacionActividades");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear actividad: {ex.Message}");
            return View("Error");
        }
    }


    [HttpPost]
    public async Task<IActionResult> EliminarActividad(int id)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Actividad WHERE ID_ACTIVIDAD = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                int result = await command.ExecuteNonQueryAsync();

                if (result > 0)
                    return Ok();
                else
                    return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar la actividad: {ex.Message}");
            return StatusCode(500, "Error interno del servidor");
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
