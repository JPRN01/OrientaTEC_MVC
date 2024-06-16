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
        Usuario usuario = ObtenerUsuarioPorCorreo(email);
        if (usuario != null && VerificarPassword(password, usuario.SaltPassword, usuario.HashedPassword))
        {
            SesionSingleton.Instance.UsuarioActual = usuario;
            //if(SesionSingleton.Instance.UsuarioActual.Rol=="Estudiante") return RedirectToAction("MenuEstudiante", "Pages");
            return RedirectToAction("MenuPrincipal", "Pages");
        }
        else
        {
            ViewBag.Error = "Credenciales inválidas";
            return View("~/Views/Home/Index.cshtml");
        }
    }

    private Usuario ObtenerUsuarioPorCorreo(string correo)
    {
        Usuario usuario = null;
        string query = @"
            SELECT nombre1, nombre2, apellido1, apellido2, correo, hashed_password, salt_password, 'Profesor' as Rol 
            FROM Profesor 
            WHERE correo = @Correo 
            UNION 
            SELECT nombre1, nombre2, apellido1, apellido2, correo, hashed_password, salt_password, 'Asistente' as Rol 
            FROM Asistente_Administrativa 
            WHERE correo = @Correo 
            UNION 
            SELECT nombre1, nombre2, apellido1, apellido2, correo, hashed_password, salt_password, 'Estudiante' as Rol 
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
            }
        }
        return usuario;
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