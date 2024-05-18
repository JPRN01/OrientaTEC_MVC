using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Data.SqlClient;

namespace OrientaTEC_MVC.Controllers
{
    public class EstudianteDAO : Controller
    {
        private readonly string connectionString;

        public EstudianteDAO()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public bool RegistrarEstudiante(Estudiante estudiante)
        {
            string query = "INSERT INTO Estudiante (Carne, Nombre1, Nombre2, Apellido1, Apellido2, Correo, Tel_Celular, centro_academico) " +
                           "VALUES (@Carne, @Nombre1, @Nombre2, @Apellido1, @Apellido2, @Correo, @TelCelular, @CentroAcademico)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Carne", estudiante.Carne);
                        command.Parameters.AddWithValue("@Nombre1", estudiante.Nombre1);
                        command.Parameters.AddWithValue("@Nombre2", estudiante.Nombre2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Apellido1", estudiante.Apellido1);
                        command.Parameters.AddWithValue("@Apellido2", estudiante.Apellido2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Correo", estudiante.Correo);
                        command.Parameters.AddWithValue("@TelCelular", estudiante.TelCelular);
                        command.Parameters.AddWithValue("@CentroAcademico", estudiante.CentroAcademico);

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
                        Console.WriteLine("Un estudiante con el mismo carne ya existe.");
                        return false;
                    }
                    throw;
                }
            }
        }

        public List<Estudiante> RecuperarEstudiantes()
        {
            List<Estudiante> estudiantes = new List<Estudiante>();
            string query = "SELECT Carne, Nombre1, Nombre2, Apellido1, Apellido2, Correo, Tel_Celular, Centro_Academico FROM Estudiante";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            estudiantes.Add(new Estudiante
                            {
                                Carne = Convert.ToInt32(reader["Carne"]),
                                Nombre1 = reader["Nombre1"].ToString(),
                                Nombre2 = reader.IsDBNull(reader.GetOrdinal("Nombre2")) ? null : reader["Nombre2"].ToString(),
                                Apellido1 = reader["Apellido1"].ToString(),
                                Apellido2 = reader.IsDBNull(reader.GetOrdinal("Apellido2")) ? null : reader["Apellido2"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                TelCelular = Convert.ToInt32(reader["Tel_Celular"]),
                                CentroAcademico = reader["Centro_Academico"].ToString(),
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return estudiantes;
        }

        public bool ModificarEstudiante(Estudiante estudiante)
        {
            string query = "UPDATE Estudiante SET Nombre1 = @Nombre1, Nombre2 = @Nombre2, Apellido1 = @Apellido1, Apellido2 = @Apellido2, " +
                           "Correo = @Correo, Tel_Celular = @TelCelular, centro_academico = @CentroAcademico WHERE Carne = @Carne";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Carne", estudiante.Carne);
                        command.Parameters.AddWithValue("@Nombre1", estudiante.Nombre1);
                        command.Parameters.AddWithValue("@Nombre2", estudiante.Nombre2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Apellido1", estudiante.Apellido1);
                        command.Parameters.AddWithValue("@Apellido2", estudiante.Apellido2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Correo", estudiante.Correo);
                        command.Parameters.AddWithValue("@TelCelular", estudiante.TelCelular);
                        command.Parameters.AddWithValue("@CentroAcademico", estudiante.CentroAcademico);

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
                        Console.WriteLine("Un estudiante con el mismo correo electrónico ya existe.");
                        return false;
                    }
                    throw;
                }
            }
        }

        public bool EliminarEstudiante(int carne)
        {
            string query = "DELETE FROM Estudiante WHERE Carne = @Carne";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Carne", carne);

                    conn.Open();
                    int rows = command.ExecuteNonQuery();
                    conn.Close();

                    return rows > 0;
                }
            }
        }
    }
}
