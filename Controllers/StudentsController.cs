using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;


namespace OrientaTEC_MVC.Controllers
{
    public class StudentsController : Controller
    {
        private NotificationDAO notificationDAO = new NotificationDAO();
        public IActionResult Students()
        {
            List<Notification> notifications = notificationDAO.GetAllNotifications();

            ViewData["Notifications"] = notifications; //PASAMOS LAS NOTIFICACIONES A LA VISTA VIEWDATA

            return View("~/Views/Pages/Students.cshtml");
        }


        public IActionResult Logout()
        {
           

            // Redirige al usuario a la página principal o a donde prefieras
            return RedirectToAction("Index", "Home");
        }

    }
}
