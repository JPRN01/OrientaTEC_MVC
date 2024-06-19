using System;

namespace OrientaTEC_MVC.Models
{
    public interface Visitor
    {
        void VisitarPublicacion(Actividad actividad);
        void VisitarRecordatorio(Actividad actividad);
    }
}
