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
            // Pobierz samoch�d z oferty, w tym dane oferty
            var car = await _context.Samochod
                .Include(c => c.Oferta) // Do��cz ofert�
                .FirstOrDefaultAsync(c => c.Id == id); // Znajd� samoch�d po Id

            if (car == null)
            {
                return NotFound(); // Zwr�� b��d 404, je�li samoch�d nie istnieje
            }

            return View(car); // Zwr�� widok z danymi samochodu
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
