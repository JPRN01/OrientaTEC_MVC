using System;
using System.Collections.Generic;
using OrientaTEC_MVC.Models;

public class NotificationDAO
{
    // Creación de algunos estados registrados para la simulación
    private static List<EstadoRegistrado> estadosRegistrados = new List<EstadoRegistrado>
    {
        new EstadoRegistrado { Id = 1, Estado = EstadoActividad.Planeada, GrabacionURL = "url1", EvidenciaURL = "url1", Justificacion = "Justificación 1" },
        new EstadoRegistrado { Id = 2, Estado = EstadoActividad.Notificada, GrabacionURL = "url2", EvidenciaURL = "url2", Justificacion = "Justificación 2" }
    };

    // Simulamos datos de Actividades
    private static List<Actividad> actividades = new List<Actividad>
    {
        new Actividad {
            IdActividad = 1,
            Nombre = "Conferencia Virtual",
            Descripcion = "Una conferencia sobre tecnología.",
            FechaExacta = DateTime.Now.AddDays(10),
            EsVirtual = true,
            ReunionURL = "http://example.com/conferencia",
            Tipo = TipoActividad.Orientadora,  // Asumiendo que Tipo es un int y corresponde a un tipo de actividad
            Estado = estadosRegistrados[0]  // Asignando un EstadoRegistrado
        },
        new Actividad {
            IdActividad = 2,
            Nombre = "Taller de Arte",
            Descripcion = "Aprende técnicas de pintura.",
            FechaExacta = DateTime.Now.AddDays(20),
            EsVirtual = false,
            Tipo = TipoActividad.Tecnica,
            Estado = estadosRegistrados[1]
        }
    };

    // Simulamos datos de Notificaciones, ahora incluyendo actividades
    private static List<Notification> notifications = new List<Notification>
    {
        new Notification { Id = 1, Title = "Bienvenida", Message = "Tu cuenta ha sido creada con éxito.", DateTime = DateTime.Now.AddHours(-2), Actividad = actividades[0] },
        new Notification { Id = 2, Title = "Recordatorio", Message = "Recuerda completar tu perfil.", DateTime = DateTime.Now.AddDays(-1), Actividad = actividades[1] }
    };

    public List<Notification> GetAllNotifications()
    {
        return notifications;
    }

    // Método para obtener una notificación específica por ID
    public Notification GetNotificationById(int id)
    {
        // Utiliza el método Find para buscar la notificación que coincida con el ID proporcionado
        return notifications.Find(notification => notification.Id == id);
    }
}
