using System;
using System.Collections.Generic;

namespace OrientaTEC_MVC.Models
{
    public class PublicacionVisitor : Visitor, Observable
    {
        private List<Observador> observadores = new List<Observador>();

        public void VisitarPublicacion(Actividad actividad)
        {
            DateTime fecha = new DateTime();
            DateTime fechaSistema = SesionSingleton.Instance.FECHA_DEL_SISTEMA;
            if (actividad.Estado.Estado == EstadoActividad.Planeada && actividad.FechaExacta <= fechaSistema)
            {
                actividad.CambiarEstado(EstadoActividad.Notificada);
                NotificarObservadores(actividad, $"Notificación: {actividad.Nombre}", $"Una nueva actividad ha sido publicada. La actividad '{actividad.Nombre}' está próxima a suceder.", fechaSistema);
            }
        }

        public void VisitarRecordatorio(Actividad actividad)
        {
            // No hace nada ya que no es su responsabilidad generar Recordatorios
        }

        public void AgregarObservador(Observador observador)
        {
            observadores.Add(observador);
        }

        public void EliminarObservador(Observador observador)
        {
            observadores.Remove(observador);
        }

        public void NotificarObservadores(Actividad actividad, string titulo, string mensaje, DateTime fecha)
        {
            foreach (var observador in observadores)
            {
                observador.Actualizar(actividad, titulo, mensaje, fecha);
            }
        }
    }
}
