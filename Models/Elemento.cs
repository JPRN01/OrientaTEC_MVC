using System;

namespace OrientaTEC_MVC.Models
{
    public interface Elemento
    {
        void Aceptar(Visitor visitor);
    }
}
