using Carente.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Dodaj ten import
using System.Threading.Tasks;

namespace Carente.Controllers
{
    public class HomeController : Controller
    {
        private readonly CarenteContext _context;

        public HomeController(CarenteContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var oferty = await _context.Oferta.ToListAsync();
            return View(oferty);
        }

        public async Task<IActionResult> Details(int id)
        {
            // Pobierz samochód z oferty, w tym dane oferty
            var car = await _context.Samochod
                .Include(c => c.Oferta) // Do³¹cz ofertê
                .FirstOrDefaultAsync(c => c.Id == id); // ZnajdŸ samochód po Id

            if (car == null)
            {
                return NotFound(); // Zwróæ b³¹d 404, jeœli samochód nie istnieje
            }

            return View(car); // Zwróæ widok z danymi samochodu
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult User()
        {
            return View();
        }
    }
}
