using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OrientaTEC_MVC.Models;

namespace OrientaTEC_MVC.Controllers
{
    public class ActividadDAO : Controller
    {
        private readonly string connectionString;

        public ActividadDAO()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public List<Actividad> ObtenerTodasLasActividades()
        {
            List<Actividad> actividades = new List<Actividad>();
            string query = "SELECT * FROM Actividad";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            actividades.Add(new Actividad
                            {
                                IdActividad = Convert.ToInt32(reader["ID_ACTIVIDAD"]),
                                Nombre = reader["nombre"].ToString(),
                                Descripcion = reader["descripcion"].ToString(),
                                Semana = Convert.ToInt32(reader["semana"]),
                                FechaExacta = Convert.ToDateTime(reader["fecha_exacta"]),
                                DiasPreviosParaAnunciar = reader["dias_previos_para_anunciar"] != DBNull.Value ? Convert.ToInt32(reader["dias_previos_para_anunciar"]) : 0,
                                DiasParaRecordar = reader["dias_para_recordar"] != DBNull.Value ? Convert.ToInt32(reader["dias_para_recordar"]) : 0,
                                EsVirtual = Convert.ToBoolean(reader["es_virtual"]),
                                ReunionURL = reader["reunion_url"].ToString(),
                                AficheURL = reader["afiche_url"].ToString(),
                                Estado = new EstadoRegistrado { Id = Convert.ToInt32(reader["ID_ESTADO_REGISTRADO"]) },
                                Tipo = (TipoActividad)Convert.ToInt32(reader["ID_TIPO_ACTIVIDAD"])
                            });
                        }
                    }
                    connection.Close();
                }
            }

            return actividades;
        }

        public Actividad ObtenerActividadPorId(int id)
        {
            Actividad actividad = null;
            string query = "SELECT * FROM Actividad WHERE ID_ACTIVIDAD = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            actividad = new Actividad
                            {
                                IdActividad = Convert.ToInt32(reader["ID_ACTIVIDAD"]),
                                Nombre = reader["nombre"].ToString(),
                                Descripcion = reader["descripcion"].ToString(),
                                Semana = Convert.ToInt32(reader["semana"]),
                                FechaExacta = Convert.ToDateTime(reader["fecha_exacta"]),
                                DiasPreviosParaAnunciar = reader["dias_previos_para_anunciar"] != DBNull.Value ? Convert.ToInt32(reader["dias_previos_para_anunciar"]) : 0,
                                DiasParaRecordar = reader["dias_para_recordar"] != DBNull.Value ? Convert.ToInt32(reader["dias_para_recordar"]) : 0,
                                EsVirtual = Convert.ToBoolean(reader["es_virtual"]),
                                ReunionURL = reader["reunion_url"].ToString(),
                                AficheURL = reader["afiche_url"].ToString(),
                                Estado = new EstadoRegistrado { Id = Convert.ToInt32(reader["ID_ESTADO_REGISTRADO"]) },
                                Tipo = (TipoActividad)Convert.ToInt32(reader["ID_TIPO_ACTIVIDAD"])
                            };
                        }
                    }
                    connection.Close();
                }
            }

            return actividad;
        }

        public bool AgregarActividad(Actividad actividad)
        {
            string query = @"INSERT INTO Actividad 
                             (nombre, descripcion, semana, fecha_exacta, dias_previos_para_anunciar, dias_para_recordar, es_virtual, reunion_url, afiche_url, ID_ESTADO_REGISTRADO, ID_TIPO_ACTIVIDAD) 
                             VALUES (@Nombre, @Descripcion, @Semana, @FechaExacta, @DiasPreviosParaAnunciar, @DiasParaRecordar, @EsVirtual, @ReunionURL, @AficheURL, @IdEstadoRegistrado, @IdTipoActividad)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Nombre", actividad.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", actividad.Descripcion);
                        command.Parameters.AddWithValue("@Semana", actividad.Semana);
                        command.Parameters.AddWithValue("@FechaExacta", actividad.FechaExacta);
                        command.Parameters.AddWithValue("@DiasPreviosParaAnunciar", actividad.DiasPreviosParaAnunciar);
                        command.Parameters.AddWithValue("@DiasParaRecordar", actividad.DiasParaRecordar);
                        command.Parameters.AddWithValue("@EsVirtual", actividad.EsVirtual);
                        command.Parameters.AddWithValue("@ReunionURL", actividad.ReunionURL);
                        command.Parameters.AddWithValue("@AficheURL", actividad.AficheURL);
                        command.Parameters.AddWithValue("@IdEstadoRegistrado", actividad.Estado.Id);
                        command.Parameters.AddWithValue("@IdTipoActividad", (int)actividad.Tipo);

                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error al agregar actividad: " + ex.Message);
                    return false;
                }
            }
        }

        public bool ActualizarActividad(Actividad actividad)
        {
            string query = @"UPDATE Actividad 
                             SET nombre = @Nombre, descripcion = @Descripcion, semana = @Semana, fecha_exacta = @FechaExacta, dias_previos_para_anunciar = @DiasPreviosParaAnunciar, dias_para_recordar = @DiasParaRecordar, es_virtual = @EsVirtual, reunion_url = @ReunionURL, afiche_url = @AficheURL, ID_ESTADO_REGISTRADO = @IdEstadoRegistrado, ID_TIPO_ACTIVIDAD = @IdTipoActividad 
                             WHERE ID_ACTIVIDAD = @IdActividad";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@IdActividad", actividad.IdActividad);
                        command.Parameters.AddWithValue("@Nombre", actividad.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", actividad.Descripcion);
                        command.Parameters.AddWithValue("@Semana", actividad.Semana);
                        command.Parameters.AddWithValue("@FechaExacta", actividad.FechaExacta);
                        command.Parameters.AddWithValue("@DiasPreviosParaAnunciar", actividad.DiasPreviosParaAnunciar);
                        command.Parameters.AddWithValue("@DiasParaRecordar", actividad.DiasParaRecordar);
                        command.Parameters.AddWithValue("@EsVirtual", actividad.EsVirtual);
                        command.Parameters.AddWithValue("@ReunionURL", actividad.ReunionURL);
                        command.Parameters.AddWithValue("@AficheURL", actividad.AficheURL);
                        command.Parameters.AddWithValue("@IdEstadoRegistrado", actividad.Estado.Id);
                        command.Parameters.AddWithValue("@IdTipoActividad", (int)actividad.Tipo);

                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error al actualizar actividad: " + ex.Message);
                    return false;
                }
            }
        }

        public bool EliminarActividad(int id)
        {
            string query = "DELETE FROM Actividad WHERE ID_ACTIVIDAD = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error al eliminar actividad: " + ex.Message);
                    return false;
                }
            }
        }

        public bool ActualizarEstado(int idActividad, EstadoActividad nuevoEstado)
        {
            string query = "UPDATE Actividad SET ID_ESTADO_REGISTRADO = @NuevoEstado WHERE ID_ACTIVIDAD = @IdActividad";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@IdActividad", idActividad);
                        command.Parameters.AddWithValue("@NuevoEstado", (int)nuevoEstado);

                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error al actualizar el estado de la actividad: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
