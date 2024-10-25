using Carente.Models;
using Microsoft.AspNetCore.Mvc; // Upewnij się, że masz tę przestrzeń nazw
using Microsoft.AspNetCore.Mvc.Rendering; // Wymagane do ViewBag
using Microsoft.EntityFrameworkCore; // Wymagane do DbContext
using System.Linq; // Potrzebne do LINQ
using System.Threading.Tasks; // Potrzebne do asynchronicznych metod

namespace YourNamespace.Controllers // Zmień na odpowiednią przestrzeń nazw dla Twojego kontrolera
{
    public class OfferController : Controller
    {
        private readonly CarenteContext _context;

        public OfferController(CarenteContext context)
        {
            _context = context;
        }

        // GET: Offer/Create
        public IActionResult Create()
        {
            var cars = _context.Samochod
                .Where(c => c.Status == "Dostepny")
                .Select(c => new
                {
                    c.Id,
                    DisplayText = $"{c.Marka} {c.Model} ({c.Rocznik})"
                })
                .ToList();

            ViewBag.Cars = new SelectList(cars, "Id", "DisplayText");
            return View("~/Views/Shared/offer_form.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Offer offer)
        {
            if (ModelState.IsValid)
            {
                // Tworzenie nowej oferty
                var oferta = new Offer
                {
                    Cena = offer.Cena,
                    Opis = offer.Opis,
                    // 'Ocena' i 'Rezerwacja_Id' są pomijane
                };

                // Dodanie oferty do bazy danych
                _context.Oferta.Add(oferta);
                await _context.SaveChangesAsync();

                // Znajdź wybrany samochód
                var car = await _context.Samochod.FindAsync(offer.SelectedCarId);
                if (car != null)
                {
                    car.Oferta_Id = oferta.Id;
                    car.Status = "Niedostępny";
                    _context.Samochod.Update(car);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            // Jeśli model nie jest prawidłowy, zwróć formularz z błędami
            var availableCars = await _context.Samochod
                .Where(c => c.Status == "Dostepny")
                .Select(c => new
                {
                    c.Id,
                    DisplayText = $"{c.Marka} {c.Model} ({c.Rocznik})"
                })
                .ToListAsync();

            ViewBag.Cars = new SelectList(availableCars, "Id", "DisplayText");
            return View("~/Views/Shared/offer_form.cshtml", offer);
        }
    }
}
