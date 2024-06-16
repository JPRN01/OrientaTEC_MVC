using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

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
        private static byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[32]; // Tamaño del salt
                rng.GetBytes(randomNumber);
                return randomNumber;
            }
        }

        // Crea el hash del password con SHA256
        public static (string, string) HashPassword(string password)
        {
            byte[] salt = GenerateSalt();
            byte[] bytes = Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt));

            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(bytes);
                string hash = Convert.ToBase64String(hashedBytes);
                string saltString = Convert.ToBase64String(salt);
                return (hash, saltString);
            }
        }

        public bool RegistrarEstudiante(Estudiante estudiante)
        {
            string query = "INSERT INTO Estudiante (CARNE, nombre1, nombre2, apellido1, apellido2, correo, tel_celular, centro_academico, hashed_password, salt_password) " +
                           "VALUES (@Carne, @Nombre1, @Nombre2, @Apellido1, @Apellido2, @Correo, @TelCelular, @CentroAcademico, @hash, @salt)";

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
                        var (hash, salt) = HashPassword(estudiante.Carne.ToString());
                        command.Parameters.AddWithValue("@hash", hash);
                        command.Parameters.AddWithValue("@salt", salt);

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
