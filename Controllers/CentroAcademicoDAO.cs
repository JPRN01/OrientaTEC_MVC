using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OrientaTEC_MVC.Controllers
{
    public class CentroAcademicoDAO : Controller
    {
        private readonly string connectionString;

        public CentroAcademicoDAO()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public bool RegistrarCentroAcademico(CentroAcademico centro)
        {
            string query = "INSERT INTO Centro_Academico (INICIALES, nombre, es_sede_central) VALUES (@Iniciales, @Nombre, @EsSedeCentral)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Iniciales", centro.Iniciales);
                        command.Parameters.AddWithValue("@Nombre", centro.Nombre);
                        command.Parameters.AddWithValue("@EsSedeCentral", centro.EsSedeCentral);

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
                        Console.WriteLine("Un centro académico con las mismas iniciales ya existe.");
                        return false;
                    }
                    throw;
                }
            }
        }

        public List<CentroAcademico> RecuperarCentrosAcademicos()
        {
            List<CentroAcademico> centros = new List<CentroAcademico>();
            string query = "SELECT INICIALES, nombre, es_sede_central FROM Centro_Academico";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            centros.Add(new CentroAcademico
                            {
                                Iniciales = reader["INICIALES"].ToString(),
                                Nombre = reader["nombre"].ToString(),
                                EsSedeCentral = Convert.ToBoolean(reader["es_sede_central"])
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return centros;
        }

        public bool ModificarCentroAcademico(CentroAcademico centro)
        {
            string query = "UPDATE Centro_Academico SET nombre = @Nombre, es_sede_central = @EsSedeCentral WHERE INICIALES = @Iniciales";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Iniciales", centro.Iniciales);
                        command.Parameters.AddWithValue("@Nombre", centro.Nombre);
                        command.Parameters.AddWithValue("@EsSedeCentral", centro.EsSedeCentral);

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
                        Console.WriteLine("Un centro académico con las mismas iniciales ya existe.");
                        return false;
                    }
                    throw;
                }
            }
        }

        public bool EliminarCentroAcademico(string iniciales)
        {
            string query = "DELETE FROM Centro_Academico WHERE INICIALES = @Iniciales";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Iniciales", iniciales);

                    conn.Open();
                    int rows = command.ExecuteNonQuery();
                    conn.Close();

                    return rows > 0;
                }
            }
        }
    }
}
