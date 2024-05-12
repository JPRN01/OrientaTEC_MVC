using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrientaTEC_MVC.Models;
using OrientaTEC_MVC.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OrientaTEC_MVC.Controllers
{
    public class ActividadesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActividadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetching data from database
            var equiposGuia = await _context.EquipoGuia
                .Select(e => new SelectListItem
                {
                    Text = $"Generación {e.Generacion}",
                    Value = e.Generacion.ToString()
                })
                .ToListAsync();

            // Ensure that we have data, if not, provide an empty list to prevent null reference
            if (equiposGuia == null || equiposGuia.Count == 0)
            {
                equiposGuia = new List<SelectListItem>
                {
                    new SelectListItem { Text = "No hay equipos disponibles", Value = "0" }
                };
            }

            var viewModel = new EquipoGuiaViewModel
            {
                EquiposGuia = equiposGuia
            };

            return View(viewModel);
        }
    }
}
