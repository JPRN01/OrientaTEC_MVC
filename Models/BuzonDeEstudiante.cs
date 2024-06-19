using OrientaTEC_MVC.Controllers;
using System;
using System.Collections.Generic;

namespace OrientaTEC_MVC.Models
{
    public class BuzonDeEstudiante : Observador
    {
        private List<Notification> mensajes = new List<Notification>();
        private NotificationDAO notificationDAO = new NotificationDAO();

        public List<Notification> Mensajes
        {
            get => mensajes;
            set => mensajes = value;
        }

        public void Actualizar(Actividad actividad, string titulo, string mensaje, DateTime fecha)
        {
            Notification nuevo_mensaje = new Notification
            {
                Title = titulo,
                Message = mensaje,
                DateTime = fecha,
                Actividad = actividad
            };
            AgregarMensaje(nuevo_mensaje);
        }

        public void AgregarMensaje(Notification mensaje)
        {
            Mensajes.Add(mensaje);
            Mensajes.Sort((x, y) => y.DateTime.CompareTo(x.DateTime));  
            notificationDAO.AddNotification(mensaje);  
        }

        public void EliminarMensaje(Notification mensaje)
        {
            Mensajes.Remove(mensaje);
            notificationDAO.DeleteNotification(mensaje.Id); 
        }
    }
}
