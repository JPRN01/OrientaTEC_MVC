using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace OrientaTEC_MVC.Controllers
{
	public class InfoController : Controller
	{
		private readonly IWebHostEnvironment _env;

		public InfoController(IWebHostEnvironment env)
		{
			_env = env;
		}

		public IActionResult Info()
		{
			return View("~/Views/Pages/Info.cshtml");
		}

		[HttpGet]
		public JsonResult GetImagenesxd()
		{
			var imagesPath = Path.Combine(_env.WebRootPath, "images", "imagenesxd");
			var allFiles = Directory.GetFiles(imagesPath).Select(Path.GetFileName).ToList();
			return Json(allFiles);
		}
	}
}
