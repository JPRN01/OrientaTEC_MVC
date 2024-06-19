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
		public IActionResult Info(int id)
		{
			var notification = notificationDAO.GetNotificationById(id);
			if (notification == null)
			{
				return NotFound(); // Si no encuentra la notificación, retorna un error 404
			}
			// Asegúrate de especificar la ruta completa a la vista
			return View("~/Views/Pages/Info.cshtml", notification); // Pasa la notificación encontrada a la vista
		}


		public IActionResult Logout()
        {
           

            // Redirige al usuario a la página principal o a donde prefieras
            return RedirectToAction("Index", "Home");
        }

    }
}
