﻿using Microsoft.AspNetCore.Mvc;
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
    a.ID_TIPO_ACTIVIDAD AS TipoId,

    STRING_AGG(p.nombre1 + ' ' + p.apellido1, ', ') AS Responsables
FROM Actividad a
JOIN Estado_Registrado e ON a.ID_ESTADO_REGISTRADO = e.ID_ESTADO_REGISTRADO
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
    a.ID_TIPO_ACTIVIDAD;

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
                        Tipo = reader["TipoId"].ToString(),
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




    [HttpPost]
    public async Task<IActionResult> EditarActividad(int IdActividad, string Nombre, string Descripcion, int Semana, bool EsVirtual, string ReunionUrl, int DiasPreviosParaAnunciar, int DiasParaRecordar, int EstadoId, int TipoId)

    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string updateQuery = @"
            UPDATE Actividad
            SET 
                nombre = @Nombre, 
                descripcion = @Descripcion, 
                semana = @Semana, 
                es_virtual = @EsVirtual, 
                reunion_url = @ReunionUrl, 
                dias_previos_para_anunciar = @DiasPreviosParaAnunciar, 
                dias_para_recordar = @DiasParaRecordar, 
                ID_ESTADO_REGISTRADO = @EstadoId, 
                ID_TIPO_ACTIVIDAD = @TipoId
            WHERE ID_ACTIVIDAD = @IdActividad";

            SqlCommand command = new SqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@IdActividad", IdActividad);
            command.Parameters.AddWithValue("@Nombre", Nombre);
            command.Parameters.AddWithValue("@Descripcion", Descripcion);
            command.Parameters.AddWithValue("@Semana", Semana);
            command.Parameters.AddWithValue("@EsVirtual", EsVirtual);
            command.Parameters.AddWithValue("@ReunionUrl", ReunionUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DiasPreviosParaAnunciar", DiasPreviosParaAnunciar);
            command.Parameters.AddWithValue("@DiasParaRecordar", DiasParaRecordar);
            command.Parameters.AddWithValue("@EstadoId", EstadoId);
            command.Parameters.AddWithValue("@TipoId", TipoId);

            try
            {
                await connection.OpenAsync();

                int result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "No se encontró la actividad para actualizar." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al editar la actividad: " + ex.Message);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }





    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> EliminarActividad(int idActividad)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command;
            try
            {
                await connection.OpenAsync();


                string deleteDependentsQuery = "DELETE FROM Profesor_X_Equipo_Guia_X_Actividad WHERE ID_ACTIVIDAD = @IdActividad";
                command = new SqlCommand(deleteDependentsQuery, connection);
                command.Parameters.AddWithValue("@IdActividad", idActividad);
                await command.ExecuteNonQueryAsync();

                // Ahora elimina la actividad
                string deleteActivityQuery = "DELETE FROM Actividad WHERE ID_ACTIVIDAD = @IdActividad";
                command = new SqlCommand(deleteActivityQuery, connection);
                command.Parameters.AddWithValue("@IdActividad", idActividad);
                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "No se encontró la actividad para eliminar." });
            }
            catch (SqlException ex)
            {
                _logger.LogError("Error al eliminar actividad: " + ex.Message);
                return Json(new { success = false, message = "Error al eliminar la actividad debido a: " + ex.Message });
            }
        }
    }



    [HttpPost]
    public async Task<IActionResult> AgregarComentario(int idActividad, string mensaje)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"
            INSERT INTO Comentario (ID_ACTIVIDAD, mensaje, emision)
            VALUES (@IdActividad, @Mensaje, GETDATE())";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdActividad", idActividad);
            command.Parameters.AddWithValue("@Mensaje", mensaje);

            try
            {
                await connection.OpenAsync();
                int result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "No se pudo agregar el comentario." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al agregar comentario: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }


    [HttpGet]
    public async Task<IActionResult> ObtenerComentarios(int idActividad)
    {
        List<ComentarioViewModel> comentarios = new List<ComentarioViewModel>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"
            SELECT mensaje, emision
            FROM Comentario
            WHERE ID_ACTIVIDAD = @IdActividad
            ORDER BY emision DESC";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdActividad", idActividad);

            try
            {
                connection.Open();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        comentarios.Add(new ComentarioViewModel
                        {
                            Mensaje = reader["mensaje"].ToString(),
                            Emision = (DateTime)reader["emision"]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener comentarios: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        return Json(comentarios);
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