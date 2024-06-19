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

        public void Actualizar()
        {
            // Lógica para actualizar el buzón
        }

        public void AgregarMensaje(Notification mensaje)
        {
            Mensajes.Add(mensaje);
        }

        public void EliminarMensaje(Notification mensaje)
        {
            Mensajes.Remove(mensaje);
        }

        public List<Notification> FiltrarMensajes(bool leidas)
        {
            return new List<Notification>();
        }
    }
}
