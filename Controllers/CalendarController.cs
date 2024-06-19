using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrientaTEC_MVC.ViewModels;
using OrientaTEC_MVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;


public class CalendarController : Controller
{

    private readonly NotificationDAO _notificationDao;


    public CalendarController()
    {
        _notificationDao = new NotificationDAO(); 
    }

    [Route("Calendar/EventCalendar")]
    public IActionResult EventCalendar()
    {
        var notifications = _notificationDao.GetAllNotifications();

        return View("~/Views/Pages/EventCalendar.cshtml", notifications);

    }

}
