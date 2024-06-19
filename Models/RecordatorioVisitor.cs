using System;
using System.Collections.Generic;

namespace OrientaTEC_MVC.Models
{
    public class RecordatorioVisitor : Visitor, Observable
    {
        private List<Observador> observadores = new List<Observador>();

        public void VisitarPublicacion(Actividad actividad, DateTime fechaSistema)
        {
            // No hace nada ya que no es su responsabilidad generar Publicaciones
        }

        public void VisitarRecordatorio(Actividad actividad, DateTime fechaSistema)
        {
            if (actividad.Estado.Estado == EstadoActividad.Notificada)
            {
                foreach (var recordatorio in actividad.Recordatorios)
                {
                    if (recordatorio.Fecha == fechaSistema)
                    {
                        // Preparar mensaje de recordatorio
                        Notification mensaje = new Notification
                        {
                            Title = $"Recordatorio: {actividad.Nombre}",
                            Message = $"La actividad '{actividad.Nombre}' está próxima a suceder.",
                            DateTime = fechaSistema,
                            Actividad = actividad
                        };

                        // Notificar observadores con el nuevo mensaje
                        //NotificarObservadores(mensaje);
                    }
                }
            }
        }

        public void AgregarObservador(Observador observador)
        {
            observadores.Add(observador);
        }

        public void EliminarObservador(Observador observador)
        {
            observadores.Remove(observador);
        }

        public void NotificarObservadores()
        {
            foreach (var observador in observadores)
            {
                observador.Actualizar();
            }
        }
    }
}
