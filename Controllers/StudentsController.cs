using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using System;
using System.Collections.Generic;

namespace OrientaTEC_MVC.Controllers
{
    public class StudentsController : Controller
    {
        private NotificationDAO notificationDAO = new NotificationDAO();
        private ActividadDAO actividadDAO = new ActividadDAO();
        private BuzonDeEstudiante buzonDeEstudiante = new BuzonDeEstudiante();
        private PublicacionVisitor publicacionVisitor = new PublicacionVisitor();
        private RecordatorioVisitor recordatorioVisitor = new RecordatorioVisitor();

        public StudentsController()
        {
            // Agregar el buzón como observador a los visitors
            publicacionVisitor.AgregarObservador(buzonDeEstudiante);
            recordatorioVisitor.AgregarObservador(buzonDeEstudiante);

            // Agregar el buzón como observador a cada actividad
            foreach (var actividad in actividadDAO.ObtenerTodasLasActividades())
            {
                actividad.AgregarObservador(buzonDeEstudiante);
            }
        }

        public IActionResult Students()
        {
            List<Notification> notifications = notificationDAO.GetAllNotifications();
            ViewData["Notifications"] = notifications;
            List<Actividad> actividades = actividadDAO.ObtenerTodasLasActividades();
            ViewData["Actividades"] = actividades;
            var usuarioActual = SesionSingleton.Instance.UsuarioActual as EstudianteDecorator;
            if (usuarioActual == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/Pages/Students.cshtml", usuarioActual);
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

        public IActionResult DeleteNotification(int id)
        {
            var dao = new NotificationDAO();
            dao.DeleteNotification(id);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateDate([FromBody] DateTimeModel model)
        {
            DateTime selectedDate = model.Date;
            SesionSingleton.Instance.FECHA_DEL_SISTEMA = selectedDate;
            ViewData["SelectedDate"] = selectedDate;

            // Actualizar las actividades con los visitors
            foreach (var actividad in actividadDAO.ObtenerTodasLasActividades())
            {
                actividad.Aceptar(publicacionVisitor);
                actividad.Aceptar(recordatorioVisitor);
            }

            List<Notification> notifications = notificationDAO.GetAllNotifications();
            ViewData["Notifications"] = notifications;

            return Ok();
        }

        public class DateTimeModel
        {
            public DateTime Date { get; set; }
        }
    }
}
