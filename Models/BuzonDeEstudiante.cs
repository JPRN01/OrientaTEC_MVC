using System.Collections.Generic;

namespace OrientaTEC_MVC.Models
{
    public class BuzonDeEstudiante: Observador
    {
        private List<Notification> mensajes;

        public List<Notification> Mensajes
        {
            get => mensajes;
            set => mensajes = value;
        }

        public void Actualizar(Actividad actividad, string titulo, string texto, DateTime fecha)
        {
            Notification nuevo_mensaje = new Notification
            {
                Title = titulo,
                Message = texto,
                DateTime = fecha,
                Actividad = actividad
            };
            AgregarMensaje(nuevo_mensaje);
        }

        public void AgregarMensaje(Notification mensaje)
        {
            Mensajes.Add(mensaje);

        }

        public void EliminarMensaje(Notification mensaje)
        {
            Mensajes.Remove(mensaje);
        }

    }
}
