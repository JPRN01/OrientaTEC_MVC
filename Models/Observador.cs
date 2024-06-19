namespace OrientaTEC_MVC.Models
{
    public interface Observador
    {
        void Actualizar(Actividad actividad, string titulo, string mensaje, DateTime fecha);
    }
}
