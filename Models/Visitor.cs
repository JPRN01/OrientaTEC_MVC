using System;

namespace OrientaTEC_MVC.Models
{
    public interface Visitor
    {
        void VisitarPublicacion(Actividad actividad, DateTime fechaSistema);
        void VisitarRecordatorio(Actividad actividad, DateTime fechaSistema);
    }
}
