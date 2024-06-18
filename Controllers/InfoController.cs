using Microsoft.AspNetCore.Mvc;

namespace OrientaTEC_MVC.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Info()
        {
            return View("~/Views/Pages/Info.cshtml");
        }
    }
}
