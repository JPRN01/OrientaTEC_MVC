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
using static OrientaTEC_MVC.ViewModels.ActividadesViewModel;

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
    SELECT DISTINCT p.nombre1, p.nombre2, p.apellido1, p.apellido2
    FROM dbo.Profesor p
    JOIN dbo.Profesor_X_Equipo_Guia pxeg ON p.NUMERO = pxeg.NUMERO AND p.CENTRO_ACADEMICO = pxeg.CENTRO_ACADEMICO
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
                            Nombre1 = reader["nombre1"].ToString(),
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


    [HttpGet]
    public async Task<IActionResult> ObtenerDetallesActividad(int idActividad)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        string query = @"
        SELECT 
            a.ID_ACTIVIDAD, 
            a.Nombre, 
            a.Descripcion, 
            a.Semana, 
            a.es_virtual, 
            a.reunion_url, 
            a.afiche_url, 
            a.dias_previos_para_anunciar, 
            a.dias_para_recordar,
            e.ID_ESTADO_ACTIVIDAD AS EstadoNombre,
            t.tipo AS TipoNombre,
            STRING_AGG(p.nombre1 + ' ' + p.apellido1, ', ') AS Responsables
        FROM Actividad a
        JOIN Estado_Registrado e ON a.ID_ESTADO_REGISTRADO = e.ID_ESTADO_REGISTRADO
        JOIN Tipo_Actividad t ON a.ID_TIPO_ACTIVIDAD = t.ID_TIPO_ACTIVIDAD
        LEFT JOIN Profesor_X_Equipo_Guia_X_Actividad pa ON a.ID_ACTIVIDAD = pa.ID_ACTIVIDAD
        LEFT JOIN Profesor p ON pa.NUMERO = p.NUMERO 
        WHERE a.ID_ACTIVIDAD = @idActividad
        GROUP BY 
            a.ID_ACTIVIDAD, 
            a.Nombre, 
            a.Descripcion, 
            a.Semana, 
            a.es_virtual, 
            a.reunion_url, 
            a.afiche_url, 
            a.dias_previos_para_anunciar, 
            a.dias_para_recordar, 
            e.ID_ESTADO_ACTIVIDAD, 
            t.tipo;
    ";

        ActividadesViewModel.ActividadDetalle actividadDetalle = null;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdActividad", idActividad);

            try
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (reader.Read())
                {
                    actividadDetalle = new ActividadesViewModel.ActividadDetalle
                    {
                        IdActividad = (int)reader["ID_ACTIVIDAD"],
                        Nombre = reader["Nombre"].ToString(),
                        Descripcion = reader["Descripcion"].ToString(),
                        Semana = (int)reader["Semana"],
                        EsVirtual = (bool)reader["es_virtual"],
                        ReunionUrl = reader["reunion_url"]?.ToString(),
                        AficheUrl = reader["afiche_url"]?.ToString(),
                        DiasPreviosParaAnunciar = (int)reader["dias_previos_para_anunciar"],
                        DiasParaRecordar = (int)reader["dias_para_recordar"],
                        Estado = reader["EstadoNombre"].ToString(),
                        Tipo = reader["TipoNombre"].ToString(),
                        Responsables = reader["Responsables"]?.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener los detalles de la actividad: {ex.Message}");
                return StatusCode(500, "Error interno del servidor: No se pudo obtener los detalles de la actividad");
            }
        }

        if (actividadDetalle == null)
        {
            return NotFound("Actividad no encontrada");
        }

        return Json(actividadDetalle); 
    }


    public async Task<IActionResult> CrearActividad(string nombre, int generacionId, string tipoId, bool esVirtual, string estadoId, string enlace, int diasPreviosParaAnunciar, int diasPreviosParaRecordar, int semana, string descripcion, IFormFile afiche, string[] profesoresNombres)
    {
        try
        {
            _logger.LogInformation($"Profesores Nombres recibidos: {string.Join(", ", profesoresNombres)}");

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

                string planInsert = "INSERT INTO Plan_Trabajo (GENERACION) OUTPUT INSERTED.ID_PLAN VALUES (@Generacion)";
                SqlCommand planCommand = new SqlCommand(planInsert, connection);
                planCommand.Parameters.AddWithValue("@Generacion", generacionId);
                int idPlan = (int)planCommand.ExecuteScalar();

           
                string actividadInsert = "INSERT INTO Actividad (nombre, ID_PLAN, ID_TIPO_ACTIVIDAD, descripcion, semana, fecha_exacta, dias_previos_para_anunciar, dias_para_recordar, es_virtual, reunion_url, afiche_url, ID_ESTADO_REGISTRADO) OUTPUT INSERTED.ID_ACTIVIDAD VALUES (@Nombre, @IdPlan, @TipoId, @Descripcion, @Semana, GETDATE(), @DiasPreviosParaAnunciar, @DiasParaRecordar, @EsVirtual, @Enlace, @AficheURL, @EstadoId)";
                SqlCommand actividadCommand = new SqlCommand(actividadInsert, connection);
                actividadCommand.Parameters.AddWithValue("@Nombre", nombre);
                actividadCommand.Parameters.AddWithValue("@IdPlan", idPlan);
                actividadCommand.Parameters.AddWithValue("@TipoId", tipoId);
                actividadCommand.Parameters.AddWithValue("@Descripcion", descripcion);
                actividadCommand.Parameters.AddWithValue("@Semana", semana);
                actividadCommand.Parameters.AddWithValue("@DiasPreviosParaAnunciar", diasPreviosParaAnunciar);
                actividadCommand.Parameters.AddWithValue("@DiasParaRecordar", diasPreviosParaRecordar);
                actividadCommand.Parameters.AddWithValue("@EsVirtual", esVirtual ? 1 : 0);
                actividadCommand.Parameters.AddWithValue("@Enlace", enlace);
                actividadCommand.Parameters.AddWithValue("@AficheURL", afichePath);
                actividadCommand.Parameters.AddWithValue("@EstadoId", estadoId);

                int idActividad = (int)actividadCommand.ExecuteScalar();

                foreach (string profesorNombre in profesoresNombres)
                {
                    string asignacionInsert = @"
                INSERT INTO Profesor_X_Equipo_Guia_X_Actividad (ID_ACTIVIDAD, NUMERO, GENERACION)
                SELECT @IdActividad, p.NUMERO, @Generacion
                FROM Profesor p
                WHERE p.nombre1 = @ProfesorNombre";

                    SqlCommand asignacionCommand = new SqlCommand(asignacionInsert, connection);
                    asignacionCommand.Parameters.AddWithValue("@IdActividad", idActividad);
                    asignacionCommand.Parameters.AddWithValue("@ProfesorNombre", profesorNombre);
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

    [HttpGet]
    public async Task<IActionResult> DetalleActividad(int idActividad)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        string query = @"
        SELECT a.ID_ACTIVIDAD, a.nombre, a.descripcion, a.semana, a.es_virtual, a.reunion_url, a.afiche_url,
               e.estado AS EstadoNombre
        FROM Actividad a
        JOIN Estado_Registrado e ON a.ID_ESTADO_REGISTRADO = e.ID_ESTADO_REGISTRADO
        WHERE a.ID_ACTIVIDAD = @IdActividad";

        ActividadesViewModel.ActividadDetalle actividadDetalle = null;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdActividad", idActividad);

            try
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (reader.Read())
                {
                    actividadDetalle = new ActividadesViewModel.ActividadDetalle
                    {
                        IdActividad = (int)reader["ID_ACTIVIDAD"],
                        Nombre = reader["nombre"].ToString(),
                        Descripcion = reader["descripcion"].ToString(),
                        Semana = (int)reader["semana"],
                        EsVirtual = (bool)reader["es_virtual"],
                        ReunionUrl = reader["reunion_url"]?.ToString(),
                        AficheUrl = reader["afiche_url"]?.ToString(),
                        Estado = reader["EstadoNombre"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener los detalles de la actividad: {ex.Message}");
                return StatusCode(500, "Internal Server Error: No se pudo obtener los detalles de la actividad");
            }
        }

        if (actividadDetalle == null)
        {
            return NotFound("Actividad no encontrada");
        }

        return View(actividadDetalle); 
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarActividad([FromBody] ActividadesViewModel.ActividadDetalle model)
    {
        List<string> updates = new List<string>();
        if (model.Nombre != null) updates.Add("nombre = @Nombre");
        if (model.Descripcion != null) updates.Add("descripcion = @Descripcion");
        if (model.Semana != 0) updates.Add("semana = @Semana");
        if (model.EsVirtual != null) updates.Add("es_virtual = @EsVirtual");
        if (model.ReunionUrl != null) updates.Add("reunion_url = @ReunionUrl");

        if (!updates.Any())
        {
            return Json(new { success = false, message = "No hay datos para actualizar" });
        }

        string setClause = string.Join(", ", updates);
        string query = $"UPDATE Actividad SET {setClause} WHERE ID_ACTIVIDAD = @IdActividad";

        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            if (model.Nombre != null) command.Parameters.AddWithValue("@Nombre", model.Nombre);
            if (model.Descripcion != null) command.Parameters.AddWithValue("@Descripcion", model.Descripcion);
            if (model.Semana != 0) command.Parameters.AddWithValue("@Semana", model.Semana);
            if (model.EsVirtual != null) command.Parameters.AddWithValue("@EsVirtual", model.EsVirtual);
            if (model.ReunionUrl != null) command.Parameters.AddWithValue("@ReunionUrl", model.ReunionUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IdActividad", model.IdActividad);

            try
            {
                connection.Open();
                int result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    return Json(new { success = true, message = "Actividad actualizada correctamente." });
                }
                return Json(new { success = false, message = "No se pudo actualizar la actividad" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la actividad: {ex.Message}");
                return StatusCode(500, "Internal Server Error: No se pudo actualizar la actividad");
            }
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

   
        ViewBag.ActividadesViewModel = actividadesViewModel ?? new ActividadesViewModel();
        return View("~/Views/Pages/PlaneacionActividades.cshtml", equipoGuiaViewModel ?? new EquipoGuiaViewModel());

    }

}
