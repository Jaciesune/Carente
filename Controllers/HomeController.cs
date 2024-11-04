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
                .Where(c => c.Oferta.Status == "Dostêpna") // tylko oferty o statusie "Dostêpna"
                .Select(c => new CarOffer
                {
                    Marka = c.Marka,
                    Rocznik = c.Rocznik,
                    Cena = (decimal)c.Oferta.Cena,
                    Oferta_Id = (int)c.Oferta_Id,
                    Zdjecie = c.Zdjecie
                })
                .ToListAsync();

            return View(carOffers);
        }



        public async Task<IActionResult> Details(int ofertaId)
        {
            var car = await _context.Samochod
                .Include(c => c.Oferta)
                .FirstOrDefaultAsync(c => c.Oferta_Id == ofertaId);

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

        [HttpPost]
        public async Task<IActionResult> DeleteOffer(int ofertaId)
        {
            var oferta = await _context.Oferta.FindAsync(ofertaId);
            if (oferta == null)
            {
                return NotFound();
            }

            var car = await _context.Samochod.FirstOrDefaultAsync(c => c.Oferta_Id == ofertaId);
            if (car != null)
            {
                car.Oferta_Id = null;
                car.Status = "Dostêpny";
            }

            _context.Oferta.Remove(oferta);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}
