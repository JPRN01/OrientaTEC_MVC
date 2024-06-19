namespace OrientaTEC_MVC.Models
{
    public class EstudianteDecorator : UsuarioDecorator
    {
        public int? NumeroTelefono { get; set; }
        public string Carnet { get; set; }
        public string Fotografia { get; set; }
        //public BuzonDeEstudiante BuzonNotificaciones { get; set; }
        public List<Actividad> ActividadesPublicadas { get; set; }

        public EstudianteDecorator(IUsuario usuario) : base(usuario)
        {
        }

        /*public void RecibirNotificacion(Mensaje mensaje)
        {
            // Lógica para recibir notificación
        }

        public void EliminarNotificacion(Mensaje mensaje)
        {
            // Lógica para eliminar notificación
        }

        public BuzonDeEstudiante FiltrarNotificaciones(bool leidas)
        {
            // Lógica para filtrar notificaciones
            return BuzonNotificaciones;
        }*/

        public Estudiante ConsultarPerfil()
        {
            // Lógica para consultar perfil
            return new Estudiante(); // Suponiendo que Estudiante es una clase que representa un perfil de estudiante
        }

        public List<Actividad> GetCalendarioActividades()
        {
            // Lógica para obtener el calendario de actividades
            return new List<Actividad>();
        }

        public Actividad VerProximaActividadPublicada()
        {
            // Lógica para ver la próxima actividad publicada
            return new Actividad();
        }
        public void ActualizarTelefono(int nuevoTelefono)
        {
            this.NumeroTelefono = nuevoTelefono;
            // Aquí debes implementar la lógica para actualizar el teléfono en la base de datos
        }

        public void ActualizarFotografia(string nuevaFotografia)
        {
            this.Fotografia = nuevaFotografia;
            // Aquí debes implementar la lógica para actualizar la URL de la fotografía en la base de datos
        }
    }

}
