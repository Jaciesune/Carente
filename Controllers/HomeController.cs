using Carente.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
            var carOffers = await _context.Samochod
                .Include(c => c.Oferta)
                .Where(c => c.Oferta.Status == "Dostêpna") // Dodaj filtr na status
                .Select(c => new CarOffer
                {
                    Id = c.Id, // Upewnij siê, ¿e przypisujesz Id
                    Marka = c.Marka,
                    Rocznik = c.Rocznik,
                    Cena = c.Oferta != null ? (decimal)c.Oferta.Cena : 0
                })
                .ToListAsync();

            return View(carOffers);
        }





        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Samochod
                .Include(c => c.Oferta)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
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
