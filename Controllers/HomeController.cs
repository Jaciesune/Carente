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
            var oferty = await _context.Oferta.ToListAsync(); // Pobierz oferty z bazy danych
            return View(oferty); // Przeka¿ oferty do widoku
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult User()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var oferta = await _context.Oferta.FindAsync(id);
            if (oferta == null)
            {
                return NotFound(); // Jeœli oferta nie zosta³a znaleziona, zwróæ 404
            }

            return View(oferta); // Zwróæ ofertê do widoku
        }

    }
}
