using Microsoft.AspNetCore.Mvc;

namespace OrientaTEC_MVC.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Logout()
        {
            // Aquí puedes limpiar la sesión del usuario, si es aplicable
            // HttpContext.Session.Clear();

            // Redirige al usuario a la página de inicio (por defecto es Index en Home)
            return RedirectToAction("Index", "Home");
        }
    }
}
