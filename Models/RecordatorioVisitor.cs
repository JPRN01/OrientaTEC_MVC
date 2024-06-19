using System;
using System.Collections.Generic;

namespace OrientaTEC_MVC.Models
{
    public class RecordatorioVisitor : Visitor, Observable
    {
        private List<Observador> observadores = new List<Observador>();

        public void VisitarPublicacion(Actividad actividad)
        {
            // No hace nada ya que no es su responsabilidad generar Publicaciones
        }

        public void VisitarRecordatorio(Actividad actividad)
        {
            DateTime fechaSistema = SesionSingleton.Instance.FECHA_DEL_SISTEMA;
            if (actividad.Estado.Estado == EstadoActividad.Notificada && actividad.FechaExacta.AddDays(-(actividad.DiasPreviosParaAnunciar)) <= fechaSistema && actividad.FechaExacta > fechaSistema)

                for (DateTime nuevoRecordatorio = actividad.FechaExacta.AddDays(-(actividad.DiasPreviosParaAnunciar)).AddDays(actividad.DiasParaRecordar); nuevoRecordatorio < actividad.FechaExacta && nuevoRecordatorio <= fechaSistema; nuevoRecordatorio=nuevoRecordatorio.AddDays(actividad.DiasParaRecordar))
                {
                    NotificarObservadores(actividad, $"Recordatorio: {actividad.Nombre}", $"La actividad '{actividad.Nombre}' está próxima a suceder.", nuevoRecordatorio);
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

        public void NotificarObservadores(Actividad actividad, string titulo, string mensaje, DateTime fecha)
        {
            foreach (var observador in observadores)
            {
                observador.Actualizar(actividad, titulo, mensaje, fecha);
            }
        }
    }
}
