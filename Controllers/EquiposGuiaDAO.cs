using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using OrientaTEC_MVC.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        
        public bool registrarEquipoGuia(EquipoGuia equipo) 
        {
            string query = "INSERT INTO Equipo_Guia VALUES (@generacion)";
            string query2 = "INSERT INTO Profesor_X_Equipo_Guia VALUES(@centro, @numero, @generacion, @coordinador, 1)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@generacion", equipo.Generacion);

                    conn.Open();

                    int rows = command.ExecuteNonQuery();

                    conn.Close();

                    if (rows == 0) return false;
                }
                try
                {
                    using (SqlCommand command = new SqlCommand(query2, conn))
                    {
                        int rows = 0;
                        int rowsAdded = 0;
                        if(equipo.Coordinador!=null)
                        {
                            rows++;
                            string centroAcademico = equipo.Coordinador.Codigo.Substring(0, 2);
                            int numero = int.Parse(equipo.Coordinador.Codigo.Substring(3));
                            command.Parameters.AddWithValue("@centro", centroAcademico);
                            command.Parameters.AddWithValue("@numero", numero);
                            command.Parameters.AddWithValue("@generacion", equipo.Generacion);
                            command.Parameters.AddWithValue("@coordinador", 1);

                            conn.Open();

                            rowsAdded += command.ExecuteNonQuery();

                            conn.Close();
                        }
                        foreach(Profesor integrante in equipo.Integrantes) 
                        {
                            rows++;
                            string centroAcademico = integrante.Codigo.Substring(0, 2);
                            int numero = int.Parse(integrante.Codigo.Substring(3));
                            command.Parameters.AddWithValue("@centro", centroAcademico);
                            command.Parameters.AddWithValue("@numero", numero);
                            command.Parameters.AddWithValue("@generacion", equipo.Generacion);
                            command.Parameters.AddWithValue("@coordinador", 0);

                            conn.Open();

                            rowsAdded += command.ExecuteNonQuery();

                            conn.Close();
                        }
                        

                        if (rows != rowsAdded) return false;
                        else return true;
                    }
                }
                catch (SqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 2627:
                        case 2601:
                            Console.WriteLine("Se intento agregar un registro ya existente en la tabla Profesor_X_Equipo_Guia.");
                            return false;
                        default:
                            throw;
                    }
                }

            }
        }
        
        public List<EquipoGuia> recuperarEquiposGuia()
        {
            List<EquipoGuia> equipos = new List<EquipoGuia>();
            List<Profesor> integrantes = new List<Profesor>();
            string query = "SELECT GENERACION FROM Equipo_Guia";
            string query2 = "SELECT p.CENTRO_ACADEMICO, p.NUMERO, p.nombre1, p.nombre2, p.apellido1, p.apellido2, p.correo, p.tel_oficina, p.tel_celular, p.imagen_url, pe.es_coordinador FROM Profesor_X_Equipo_Guia pe INNER JOIN Profesor p ON p.CENTRO_ACADEMICO = pe.CENTRO_ACADEMICO AND p.NUMERO = pe.NUMERO WHERE pe.GENERACION=@generacion";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            equipos.Add(new EquipoGuia
                            {
                                Generacion = Convert.ToInt32(reader["GENERACION"].ToString()),
                            });
                        }
                    }
                    connection.Close();
                }
                foreach (EquipoGuia equipo in equipos)
                {
                    using (SqlCommand command = new SqlCommand(query2, connection))
                    {
                        command.Parameters.AddWithValue("@generacion", equipo.Generacion);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["es_coordinador"].ToString() == "True")
                                {
                                    Profesor coordinador = new Profesor
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
                                    };
                                    equipo.Coordinador = coordinador;
                                }
                                else 
                                {
                                    integrantes.Add(new Profesor
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
                                    });
                                }
                                equipo.Integrantes = integrantes;
                            }
                        }
                        connection.Close();
                    }
                }
            }
            return equipos;
        }
    }
}
