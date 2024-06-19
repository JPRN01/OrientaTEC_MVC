// SesionController.cs
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using Microsoft.Extensions.Configuration;

public class SesionController : Controller
{
    private readonly string connectionString;

    public SesionController()
    {
        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        connectionString = config.GetConnectionString("DefaultConnection");
    }

    [HttpPost]
    public IActionResult IniciarSesion(string email, string password)
    {
        var (usuario, numeroTelefono, fotografia, carne) = ObtenerUsuarioPorCorreo(email);
        if (usuario != null && VerificarPassword(password, usuario.SaltPassword, usuario.HashedPassword))
        {
            if (usuario.Rol == "Estudiante")
            {
                var estudianteDecorator = new EstudianteDecorator(usuario)
                {
                    NumeroTelefono = numeroTelefono,
                    Fotografia = fotografia,
                    Carnet = carne
                };
                SesionSingleton.Instance.UsuarioActual = estudianteDecorator;
            }
            else
            {
                SesionSingleton.Instance.UsuarioActual = usuario;
            }

            if (usuario.Rol == "Estudiante") return RedirectToAction("Students", "Pages");
            return RedirectToAction("MenuPrincipal", "Pages");
        }
        else
        {
            ViewBag.Error = "Credenciales inválidas";
            return View("~/Views/Home/Index.cshtml");
        }
    }

    private (IUsuario, int?, string, string) ObtenerUsuarioPorCorreo(string correo)
    {
        IUsuario usuario = null;
        int? numeroTelefono = null;
        string fotografia = null;
        string carne = null;

        string query = @"
            SELECT nombre1, nombre2, apellido1, apellido2, correo, hashed_password, salt_password, 'Profesor' as Rol, NULL as tel_celular, NULL as imagen_url, NULL as CARNE
            FROM Profesor 
            WHERE correo = @Correo 
            UNION 
            SELECT nombre1, nombre2, apellido1, apellido2, correo, hashed_password, salt_password, 'Asistente' as Rol, NULL as tel_celular, NULL as imagen_url, NULL as CARNE
            FROM Asistente_Administrativa 
            WHERE correo = @Correo 
            UNION 
            SELECT nombre1, nombre2, apellido1, apellido2, correo, hashed_password, salt_password, 'Estudiante' as Rol, tel_celular, imagen_url, CARNE
            FROM Estudiante 
            WHERE correo = @Correo";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Correo", correo);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                usuario = new Usuario
                {
                    Nombre1 = reader["nombre1"].ToString(),
                    Nombre2 = reader["nombre2"].ToString(),
                    Apellido1 = reader["apellido1"].ToString(),
                    Apellido2 = reader["apellido2"].ToString(),
                    Correo = reader["correo"].ToString(),
                    HashedPassword = reader["hashed_password"].ToString(),
                    SaltPassword = reader["salt_password"].ToString(),
                    Rol = reader["Rol"].ToString()
                };
                if (usuario.Rol == "Estudiante")
                {
                    numeroTelefono = reader["tel_celular"] != DBNull.Value ? Convert.ToInt32(reader["tel_celular"]) : (int?)null;
                    fotografia = reader["imagen_url"]?.ToString();
                    carne = reader["CARNE"]?.ToString();
                }
            }
        }
        return (usuario, numeroTelefono, fotografia, carne);
    }

    private bool VerificarPassword(string passwordIngresado, string salt, string hashedPassword)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(passwordIngresado + salt);
        using (var sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(bytes);
            string hash = Convert.ToBase64String(hashedBytes);
            return hash == hashedPassword;
        }
    }
}
