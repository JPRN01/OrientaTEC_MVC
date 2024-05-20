using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OrientaTEC_MVC.Controllers
{
    public class EquiposGuiaDAO : Controller
    {
        private readonly string connectionString;

        public EquiposGuiaDAO()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public bool RegistrarEquipoGuia(EquipoGuia equipoGuia)
        {
            string queryInsertEquipo = "INSERT INTO Equipo_Guia (GENERACION) VALUES (@Generacion)";
            string queryAsignarCoordinador = "EXEC sp_AsignarCoordinador @CENTRO_ACADEMICO, @NUMERO, @GENERACION";
            string queryAgregarIntegrante = "EXEC sp_AgregarIntegrante @CENTRO_ACADEMICO, @NUMERO, @GENERACION";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        
                        using (SqlCommand command = new SqlCommand(queryInsertEquipo, conn, transaction))
                        {
                            command.Parameters.AddWithValue("@Generacion", equipoGuia.Generacion);
                            command.ExecuteNonQuery();
                        }

                        

                      
                        foreach (var integrante in equipoGuia.Integrantes)
                        {
                            using (SqlCommand command = new SqlCommand(queryAgregarIntegrante, conn, transaction))
                            {
                                command.Parameters.AddWithValue("@CENTRO_ACADEMICO", integrante.Sede);
                                command.Parameters.AddWithValue("@NUMERO", int.Parse(integrante.Codigo.Split('-')[1]));
                                command.Parameters.AddWithValue("@GENERACION", equipoGuia.Generacion);
                                command.ExecuteNonQuery();
                            }
                        }

                        if (equipoGuia.Coordinador != null)
                        {
                            using (SqlCommand command = new SqlCommand(queryAsignarCoordinador, conn, transaction))
                            {
                                command.Parameters.AddWithValue("@CENTRO_ACADEMICO", equipoGuia.Coordinador.Sede);
                                command.Parameters.AddWithValue("@NUMERO", int.Parse(equipoGuia.Coordinador.Codigo.Split('-')[1]));
                                command.Parameters.AddWithValue("@GENERACION", equipoGuia.Generacion);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error al registrar el equipo guía: " + ex.Message);
                    return false;
                }
            }
        }


        public List<EquipoGuia> RecuperarEquiposGuia()
        {
            List<EquipoGuia> equiposGuia = new List<EquipoGuia>();
            string query = "SELECT GENERACION FROM Equipo_Guia";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            equiposGuia.Add(new EquipoGuia
                            {
                                Generacion = Convert.ToInt32(reader["GENERACION"])
                            });
                        }
                    }
                    connection.Close();
                }
            }

            foreach (var equipoGuia in equiposGuia)
            {
                equipoGuia.Coordinador = ObtenerCoordinador(equipoGuia.Generacion);
                equipoGuia.Integrantes = ObtenerIntegrantes(equipoGuia.Generacion);
            }

            return equiposGuia;
        }


        private Profesor ObtenerCoordinador(int generacion)
        {
            Profesor coordinador = null;
            string query = @"
                SELECT p.CENTRO_ACADEMICO, p.NUMERO, p.nombre1, p.nombre2, p.apellido1, p.apellido2, p.correo
                FROM Profesor p
                JOIN Profesor_X_Equipo_Guia PxEG ON p.CENTRO_ACADEMICO = PxEG.CENTRO_ACADEMICO AND p.NUMERO = PxEG.NUMERO
                WHERE PxEG.GENERACION = @Generacion AND PxEG.es_coordinador = 1 AND PxEG.esta_activo = 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Generacion", generacion);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            coordinador = new Profesor
                            {
                                Sede = reader["CENTRO_ACADEMICO"].ToString(),
                                Codigo = reader["CENTRO_ACADEMICO"].ToString() + "-" + reader["NUMERO"].ToString(),
                                Nombre1 = reader["nombre1"].ToString(),
                                Nombre2 = reader.IsDBNull(reader.GetOrdinal("nombre2")) ? null : reader["nombre2"].ToString(),
                                Apellido1 = reader["apellido1"].ToString(),
                                Apellido2 = reader.IsDBNull(reader.GetOrdinal("apellido2")) ? null : reader["apellido2"].ToString(),
                                Correo = reader["correo"].ToString()
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return coordinador;
        }

        private List<Profesor> ObtenerIntegrantes(int generacion)
        {
            List<Profesor> integrantes = new List<Profesor>();
            string query = @"
                SELECT p.CENTRO_ACADEMICO, p.NUMERO, p.nombre1, p.nombre2, p.apellido1, p.apellido2, p.correo
                FROM Profesor p
                JOIN Profesor_X_Equipo_Guia PxEG ON p.CENTRO_ACADEMICO = PxEG.CENTRO_ACADEMICO AND p.NUMERO = PxEG.NUMERO
                WHERE PxEG.GENERACION = @Generacion AND PxEG.es_coordinador = 0 AND PxEG.esta_activo = 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Generacion", generacion);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            integrantes.Add(new Profesor
                            {
                                Sede = reader["CENTRO_ACADEMICO"].ToString(),
                                Codigo = reader["CENTRO_ACADEMICO"].ToString() + "-" + reader["NUMERO"].ToString(),
                                Nombre1 = reader["nombre1"].ToString(),
                                Nombre2 = reader.IsDBNull(reader.GetOrdinal("nombre2")) ? null : reader["nombre2"].ToString(),
                                Apellido1 = reader["apellido1"].ToString(),
                                Apellido2 = reader.IsDBNull(reader.GetOrdinal("apellido2")) ? null : reader["apellido2"].ToString(),
                                Correo = reader["correo"].ToString()
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return integrantes;
        }

        public bool AsignarCoordinador(int generacion, Profesor profesor)
        {
            string query = @"EXEC sp_AsignarCoordinador @CENTRO_ACADEMICO, @NUMERO, @GENERACION";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@CENTRO_ACADEMICO", profesor.Sede);
                        command.Parameters.AddWithValue("@Numero", int.Parse(profesor.Codigo.Split('-')[1]));
                        command.Parameters.AddWithValue("@Generacion", generacion);
                        conn.Open();
                        int rows = command.ExecuteNonQuery();
                        conn.Close();
                        return rows > 0;
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        Console.WriteLine("La relación ya existe.");
                        return false;
                    }
                    throw;
                }
            }
        }


        public bool AgregarIntegrante(int generacion, Profesor profesor)
        {
            string query = @"EXEC sp_AgregarIntegrante @CENTRO_ACADEMICO, @NUMERO, @GENERACION";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@CENTRO_ACADEMICO", profesor.Sede);
                        command.Parameters.AddWithValue("@Numero", int.Parse(profesor.Codigo.Split('-')[1]));
                        command.Parameters.AddWithValue("@Generacion", generacion);
                        conn.Open();
                        int rows = command.ExecuteNonQuery();
                        conn.Close();
                        return rows > 0;
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        Console.WriteLine("La relación ya existe.");
                        return false;
                    }
                    throw;
                }
            }
        }


        public bool EliminarIntegrante(int generacion, string codigoProfesor)
        {
            string query = @"UPDATE Profesor_X_Equipo_Guia SET esta_activo=0 WHERE GENERACION = @Generacion AND CENTRO_ACADEMICO = @CentroAcademico AND NUMERO = @Numero";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        string[] parts = codigoProfesor.Split('-');
                        string centroAcademico = parts[0];
                        int numero = int.Parse(parts[1]);

                        command.Parameters.AddWithValue("@Generacion", generacion);
                        command.Parameters.AddWithValue("@CentroAcademico", centroAcademico);
                        command.Parameters.AddWithValue("@Numero", numero);

                        conn.Open();
                        int rows = command.ExecuteNonQuery();
                        conn.Close();

                        return rows > 0;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error al eliminar el integrante: " + ex.Message);
                    return false;
                }
            }
        }



    }
}
