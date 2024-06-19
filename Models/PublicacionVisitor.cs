using System;
using System.Collections.Generic;

namespace OrientaTEC_MVC.Models
{
    public class PublicacionVisitor : Visitor, Observable
    {
        private List<Observador> observadores = new List<Observador>();

        public void VisitarPublicacion(Actividad actividad, DateTime fechaSistema)
        {
            // Implementación de la lógica de visita de publicación
        }

        public void VisitarRecordatorio(Actividad actividad, DateTime fechaSistema)
        {
            // Implementación de la lógica de visita de recordatorio
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
