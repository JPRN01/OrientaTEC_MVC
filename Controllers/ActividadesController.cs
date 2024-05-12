using Microsoft.AspNetCore.Mvc;
using OrientaTEC_MVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrientaTEC_MVC.ViewModels;

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
            var equiposGuia = await _context.EquipoGuia
                .Select(e => new SelectListItem
                {
                    Text = $"Generación {e.Generacion}", // Simplificado sin coordinador
                    Value = e.Generacion.ToString()
                })
                .ToListAsync();

            var viewModel = new EquipoGuiaViewModel
            {
                EquiposGuia = equiposGuia
            };

            return View(viewModel);
        }
    }
}
