using System;
using System.Collections.Generic;
using OrientaTEC_MVC.Models;

public class NotificationDAO
{

    //SIMULAMOS
    private static List<Notification> notifications = new List<Notification>
    {
        new Notification { Id = 1, Title = "Bienvenida", Message = "Tu cuenta ha sido creada con éxito.", DateTime = DateTime.Now.AddHours(-2) },
        new Notification { Id = 2, Title = "Recordatorio", Message = "Recuerda completar tu perfil.", DateTime = DateTime.Now.AddDays(-1) }
    };


    public List<Notification> GetAllNotifications()
    {
        return notifications;
    }
}