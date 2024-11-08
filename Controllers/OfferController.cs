using Carente.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Carente.Controllers
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
                // Tworzenie i zapis nowej oferty
                var newOffer = new Offer
                {
                    Cena = offer.Cena,
                    Opis = offer.Opis,
                };

                // Dodanie nowej oferty do bazy danych
                _context.Oferta.Add(newOffer);
                await _context.SaveChangesAsync();

                // Znalezienie i zaktualizowanie wybranego samochodu
                var car = await _context.Samochod.FindAsync(offer.SelectedCarId);
                if (car != null)
                {
                    car.Oferta_Id = newOffer.Id;
                    car.Status = "Niedostępny";
                    _context.Samochod.Update(car);
                    await _context.SaveChangesAsync();
                }

                // Przekierowanie na stronę główną
                return RedirectToAction("Index", "Home");
            }

            // Jeśli występują błędy, ponownie wyświetl formularz z dostępnymi samochodami
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

        public async Task<IActionResult> Reserve(int carId)
        {
            var disabledDates = await _context.Rezerwacja
                .Where(r => r.Samochod_Id == carId)
                .SelectMany(r => Enumerable.Range(0, 1 + (r.Data_Zakonczenia - r.Data_Rozpoczecia).Days)
                                           .Select(offset => r.Data_Rozpoczecia.AddDays(offset)))
                .ToListAsync();

            ViewBag.DisabledDates = disabledDates;
            return View("offer_form"); // Adjust if a different view is used
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(int carId, DateTime reservationStart, DateTime reservationEnd, float cena)
        {
            if (reservationEnd <= reservationStart)
            {
                ModelState.AddModelError("", "Data zakończenia musi być późniejsza niż data rozpoczęcia.");
                return View("offer_form"); // Re-render form with error
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Obsłuż sytuację, gdy użytkownik nie jest zalogowany
                return RedirectToAction("Login", "Account");
            }

            var newReservation = new Rezerwacja
            {
                Uzytkownik_Id = userId.Value, // Ustawia ID użytkownika z sesji
                Samochod_Id = carId,
                Data_Rozpoczecia = reservationStart,
                Data_Zakonczenia = reservationEnd,
                Cena = cena
            };

            _context.Rezerwacja.Add(newReservation);

            var car = await _context.Samochod.FindAsync(carId);
            if (car != null)
            {
                car.Status = "Zarezerwowany";
                _context.Samochod.Update(car);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
