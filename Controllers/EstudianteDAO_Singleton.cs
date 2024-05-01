using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace OrientaTEC_MVC.Controllers
{
    public class EstudianteDAO_Singleton : Controller
    {
        private static EstudianteDAO_Singleton _instance;
        private static readonly object _lock = new object();
        private readonly string connectionString;

        private EstudianteDAO_Singleton()
        {
            // Asignar la cadena de conexión desde appsettings.json
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public static EstudianteDAO_Singleton Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new EstudianteDAO_Singleton();
                    }
                    return _instance;
                }
            }
        }

        public bool AgregarEstudiante(Estudiante estudiante)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                const string query = @"
                INSERT INTO Estudiante (CARNE, nombre1, nombre2, apellido1, apellido2, correo, tel_celular, centro_academico)
                VALUES (@Carnet, @Nombre1, @Nombre2, @Apellido1, @Apellido2, @Correo, @TelCelular, @CentroAcademico)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Carnet", estudiante.Carnet);
                    command.Parameters.AddWithValue("@Nombre1", estudiante.Nombre1);
                    command.Parameters.AddWithValue("@Nombre2", estudiante.Nombre2 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Apellido1", estudiante.Apellido1);
                    command.Parameters.AddWithValue("@Apellido2", estudiante.Apellido2);
                    command.Parameters.AddWithValue("@Correo", estudiante.Correo);
                    command.Parameters.AddWithValue("@TelCelular", estudiante.TelCelular);
                    command.Parameters.AddWithValue("@CentroAcademico", estudiante.CentroAcademico);

                    try
                    {
                        connection.Open();
                        var result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                    catch (SqlException)
                    {
                        return false;
                    }
                }
            }
        }
    }
}
