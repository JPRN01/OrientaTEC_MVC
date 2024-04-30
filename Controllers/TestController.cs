using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrientaTEC_MVC; // Correcto si SomeEntity está aquí
using System.Linq;
using System.Threading.Tasks;

namespace OrientaTEC_MVC.Controllers
{
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Insertar un nuevo elemento
                _context.SomeEntities.Add(new SomeEntity { Name = "Test Name" });
                await _context.SaveChangesAsync();

                // Leer el elemento insertado
                var item = await _context.SomeEntities.FirstOrDefaultAsync();
                return View(item);  // Pasar el item a la vista
            }
            catch (Exception ex)
            {
                // Si ocurre un error, captura la excepción interna si está presente
                string innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return Content($"Error: {ex.Message}, Inner Exception: {innerExceptionMessage}");
            }
        }
    }
}
