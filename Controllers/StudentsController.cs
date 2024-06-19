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

            ViewData["Notifications"] = notifications; 

            return View("~/Views/Pages/Students.cshtml");
        }
		public IActionResult Info(int id)
		{
			var notification = notificationDAO.GetNotificationById(id);
			if (notification == null)
			{
				return NotFound(); 
			}
		
			return View("~/Views/Pages/Info.cshtml", notification); 
		}


		public IActionResult Logout()
        {
           
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public IActionResult MarkAsViewed(int id)
        {
            notificationDAO.MarkAsViewed(id); 
            return Ok();
        }







        [HttpPost]
        public IActionResult UpdateDate([FromBody] DateTimeModel model)
        {
            DateTime selectedDate = model.Date;

            SesionSingleton.Instance.FECHA_DEL_SISTEMA = selectedDate;

            ViewData["SelectedDate"] = selectedDate;

            return Ok();
        }

        public class DateTimeModel
        {
            public DateTime Date { get; set; }
        }



    }
}
