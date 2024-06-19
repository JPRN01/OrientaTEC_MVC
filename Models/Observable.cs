namespace OrientaTEC_MVC.Models
{
    public interface Observable
    {
        void AgregarObservador(Observador observador);
        void EliminarObservador(Observador observador);
        void NotificarObservadores();
    }
}
