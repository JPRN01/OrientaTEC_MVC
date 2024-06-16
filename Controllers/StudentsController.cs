using Microsoft.AspNetCore.Mvc;

namespace OrientaTEC_MVC.Controllers
{
    public class StudentsController : Controller
    {
        public IActionResult Students()
        {
            return View("~/Views/Pages/Students.cshtml");
        }
    }
}
