using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace OrientaTEC_MVC.Controllers
{
    public class PagesController : Controller
    {
        private readonly ILogger<PagesController> _logger;
        private readonly string connectionString;

        public PagesController(ILogger<PagesController> logger)
        {
            _logger = logger;
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public IActionResult GestorEquiposGuia()
        {
            return View();
        }

        public IActionResult GestorEstudiantes()
        {
            return View();
        }

        public IActionResult MenuPrincipal()
        {
            return View();
        }

        public IActionResult Students()
        {
            var usuarioActual = SesionSingleton.Instance.UsuarioActual as EstudianteDecorator;
            if (usuarioActual == null)
            {
                return RedirectToAction("Home", "Index");
            }
            return View(usuarioActual);
        }

        public IActionResult VerPerfil()
        {
            var usuarioActual = SesionSingleton.Instance.UsuarioActual as EstudianteDecorator;
            if (usuarioActual == null)
            {
                return RedirectToAction("Home", "Index");
            }
            return View(usuarioActual);
        }

        [HttpPost]
        public IActionResult ModificarContrasena(string currentPassword, string newPassword, string confirmNewPassword)
        {
            var usuarioActual = SesionSingleton.Instance.UsuarioActual as EstudianteDecorator;
            if (usuarioActual == null)
            {
                return RedirectToAction("Home", "Index");
            }

            if (newPassword != confirmNewPassword)
            {
                ViewBag.Error = "Las nuevas contrase�as no coinciden.";
                return View("VerPerfil", usuarioActual);
            }

            if (!VerificarPassword(currentPassword, usuarioActual.SaltPassword, usuarioActual.HashedPassword))
            {
                ViewBag.Error = "La contrase�a actual es incorrecta.";
                return View("VerPerfil", usuarioActual);
            }

            string newSalt = GenerateSalt();
            string newHashedPassword = HashPassword(newPassword, newSalt);

            string query = "UPDATE Estudiante SET hashed_password = @hash, salt_password = @salt WHERE Carne = @Carne";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Carne", usuarioActual.Carnet);
                    command.Parameters.AddWithValue("@hash", newHashedPassword);
                    command.Parameters.AddWithValue("@salt", newSalt);

                    conn.Open();
                    int rows = command.ExecuteNonQuery();
                    conn.Close();
                }
            }

            usuarioActual.SaltPassword = newSalt;
            usuarioActual.HashedPassword = newHashedPassword;

            ViewBag.Message = "Contrase�a actualizada con �xito.";
            return View("VerPerfil", usuarioActual);
        }

        [HttpPost]
        public IActionResult ModificarTelefono(int nuevoTelefono)
        {
            var usuarioActual = SesionSingleton.Instance.UsuarioActual as EstudianteDecorator;
            if (usuarioActual == null)
            {
                return RedirectToAction("Home", "Index");
            }

            usuarioActual.ActualizarTelefono(nuevoTelefono);

            string query = "UPDATE Estudiante SET tel_celular = @tel_celular WHERE Carne = @Carne";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Carne", usuarioActual.Carnet);
                    command.Parameters.AddWithValue("@tel_celular", nuevoTelefono);

                    conn.Open();
                    int rows = command.ExecuteNonQuery();
                    conn.Close();
                }
            }

            ViewBag.Message = "N�mero de tel�fono actualizado con �xito.";
            return View("VerPerfil", usuarioActual);
        }

        [HttpPost]
        public IActionResult ModificarFotografia(IFormFile nuevaFotografia)
        {
            var usuarioActual = SesionSingleton.Instance.UsuarioActual as EstudianteDecorator;
            if (usuarioActual == null)
            {
                return RedirectToAction("Home", "Index");
            }

            if (nuevaFotografia != null && nuevaFotografia.Length > 0)
            {
                var fileName = Path.GetFileName(nuevaFotografia.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                // Aseg�rate de que el directorio exista
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    nuevaFotografia.CopyTo(stream);
                }

                usuarioActual.ActualizarFotografia("/uploads/" + fileName);

                string query = "UPDATE Estudiante SET imagen_url = @imagen_url WHERE Carne = @Carne";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Carne", usuarioActual.Carnet);
                        command.Parameters.AddWithValue("@imagen_url", "/uploads/" + fileName);

                        conn.Open();
                        int rows = command.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }

            ViewBag.Message = "Fotograf�a actualizada con �xito.";
            return View("VerPerfil", usuarioActual);
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

        private string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password + salt);
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public IActionResult PlaneacionActividades()
        {
            return View();
        }
    }
}
