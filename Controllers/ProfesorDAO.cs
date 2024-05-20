using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrientaTEC_MVC.Controllers
{
    public class ProfesorDAO : Controller
    {
        private readonly string connectionString;

        public ProfesorDAO()
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

        public bool registrarProfesorGuia(Profesor profesor)
        {
            int lastId = 1;
            string query = "SELECT TOP 1 NUMERO FROM Profesor WHERE CENTRO_ACADEMICO = @CentroAcademico ORDER BY NUMERO DESC";
            string query2 = "INSERT INTO Profesor VALUES (@CentroAcademico, @NUMERO, @nombre1,@nombre2, @apellido1,@apellido2, @correo, @hash, @salt, @oficina, @celular, @imagen, 1)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@CentroAcademico", profesor.Sede);

                    conn.Open();

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        lastId = Convert.ToInt32(result);
                    }

                    conn.Close();
                }
                try
                {
                    using (SqlCommand command = new SqlCommand(query2, conn))
                    {
                        command.Parameters.AddWithValue("@CentroAcademico", profesor.Sede);
                        command.Parameters.AddWithValue("@NUMERO", lastId + 1);
                        command.Parameters.AddWithValue("@nombre1", profesor.Nombre1);
                        command.Parameters.AddWithValue("@nombre2", profesor.Nombre2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@apellido1", profesor.Apellido1);
                        command.Parameters.AddWithValue("@apellido2", profesor.Apellido2);
                        command.Parameters.AddWithValue("@correo", profesor.Correo);
                        var (hash, salt) = HashPassword(profesor.Contrasena);
                        command.Parameters.AddWithValue("@hash", hash);
                        command.Parameters.AddWithValue("@salt", salt);
                        command.Parameters.AddWithValue("@oficina", string.IsNullOrEmpty(profesor.TelOficina) ? DBNull.Value : (object)profesor.TelOficina);
                        command.Parameters.AddWithValue("@celular", profesor.TelCelular ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@imagen", profesor.ImagenURL ?? (object)DBNull.Value);

                        conn.Open();

                        int rows = command.ExecuteNonQuery();

                        conn.Close();

                        if (rows == 0) return false;
                        else return true;
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        Console.WriteLine("Un profesor con el mismo correo electrónico ya existe.");
                        return false;
                    }
                    throw;
                }

            }
        }
        public List<Profesor> recuperarProfesores()
        {
            List<Profesor> profesores = new List<Profesor>();
            string query = "SELECT CENTRO_ACADEMICO, NUMERO, nombre1, nombre2, apellido1, apellido2, correo, tel_oficina, tel_celular, imagen_url, esta_activo FROM Profesor";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            profesores.Add(new Profesor
                            {
                                Sede = reader["CENTRO_ACADEMICO"].ToString(),
                                Codigo = reader["CENTRO_ACADEMICO"].ToString() + "-" + reader["NUMERO"].ToString(),
                                Nombre1 = reader["nombre1"].ToString(),
                                Nombre2 = reader.IsDBNull(reader.GetOrdinal("nombre2")) ? null : reader["nombre2"].ToString(),
                                Apellido1 = reader["apellido1"].ToString(),
                                Apellido2 = reader.IsDBNull(reader.GetOrdinal("apellido2")) ? null : reader["apellido2"].ToString(),
                                Correo = reader["correo"].ToString(),
                                TelOficina = reader.IsDBNull(reader.GetOrdinal("tel_oficina")) ? null : reader["tel_oficina"].ToString(),
                                TelCelular = reader.IsDBNull(reader.GetOrdinal("tel_celular")) ? null : Convert.ToInt32(reader["tel_celular"].ToString()),
                                ImagenURL = reader.IsDBNull(reader.GetOrdinal("imagen_URL")) ? null : reader["imagen_URL"].ToString(),
                                Activo = reader.IsDBNull(reader.GetOrdinal("esta_activo")) ? false : reader["esta_activo"].ToString() == "True"
                            });
                        }
                    }
                    connection.Close();
                }
            }

            //foreach (Profesor profesor in profesores)
            //{
            //    bool isActive;
            //    string centroAcademico = profesor.Codigo.Substring(0, 2);
            //    int numero = int.Parse(profesor.Codigo.Substring(3));
            //    string query2 = @"
            //    SELECT CASE WHEN EXISTS (
            //        SELECT 1
            //        FROM Profesor_X_Equipo_Guia
            //        WHERE CENTRO_ACADEMICO = @CentroAcademico
            //          AND NUMERO = @Numero
            //          AND esta_Activo = 1
            //    ) THEN 1 ELSE 0 END";

            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //    {
            //        using (SqlCommand command = new SqlCommand(query2, connection))
            //        {
            //            command.Parameters.AddWithValue("@CentroAcademico", centroAcademico);
            //            command.Parameters.AddWithValue("@Numero", numero);

            //            connection.Open();
            //            isActive = (int)command.ExecuteScalar() == 1;
            //            connection.Close();
            //        }
            //    }
            //    profesor.Activo = isActive;
            //}

            return profesores;
        }
        public void darBajaProfesorGuia(string id, bool activo)
        {
            string query = "UPDATE Profesor SET esta_activo = @estado WHERE CENTRO_ACADEMICO = @centro AND NUMERO = @numero";
            string centroAcademico = id.Substring(0, 2);
            int numero = int.Parse(id.Substring(3));
            int estado = activo ? 1 : 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@estado", estado);
                    command.Parameters.AddWithValue("@centro", centroAcademico);
                    command.Parameters.AddWithValue("@numero", numero);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public bool modificarProfesorGuia(Profesor profesor)
        {
            string centroAcademico = profesor.Codigo.Substring(0, 2);
            int numero = int.Parse(profesor.Codigo.Substring(3));
            string query = "UPDATE Profesor SET nombre1 = @nombre1,nombre2 = @nombre2, apellido1 = @apellido1,apellido2 = @apellido2,correo = @correo,tel_oficina = @oficina, tel_celular=@celular,imagen_url = @imagen WHERE CENTRO_ACADEMICO = @CentroAcademico AND NUMERO = @NUMERO";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@CentroAcademico", centroAcademico);
                        command.Parameters.AddWithValue("@NUMERO", numero);
                        command.Parameters.AddWithValue("@nombre1", profesor.Nombre1);
                        command.Parameters.AddWithValue("@nombre2", profesor.Nombre2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@apellido1", profesor.Apellido1);
                        command.Parameters.AddWithValue("@apellido2", profesor.Apellido2);
                        command.Parameters.AddWithValue("@correo", profesor.Correo);
                        command.Parameters.AddWithValue("@oficina", string.IsNullOrEmpty(profesor.TelOficina) ? DBNull.Value : (object)profesor.TelOficina);
                        command.Parameters.AddWithValue("@celular", profesor.TelCelular ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@imagen", profesor.ImagenURL ?? (object)DBNull.Value);

                        conn.Open();

                        int rows = command.ExecuteNonQuery();

                        conn.Close();

                        if (rows == 0) return false;
                        else return true;
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        Console.WriteLine("Un profesor con el mismo correo electrónico ya existe.");
                        return false;
                    }
                    throw;
                }

            }
        }
    }
}
