using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Carente.Models;

namespace Carente.Controllers
{
    public class RezerwacjeController : Controller
    {
        private readonly CarenteContext _context;

        public RezerwacjeController(CarenteContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var rezerwacje = _context.Rezerwacja
                .Where(r => r.Uzytkownik_Id == userId)
                .Join(_context.Samochod,
                      rez => rez.Samochod_Id,
                      car => car.Id,
                      (rez, car) => new
                      {
                          rezerwacja = rez,
                          car = car,
                          offer = car.Oferta
                      })
                .Select(result => new RezerwacjaViewModel
                {
                    Id = result.rezerwacja.Id,
                    Marka = result.car.Marka,
                    Model = result.car.Model,
                    Cena = (decimal)result.rezerwacja.Cena + (decimal)result.offer.Cena, // Cena oferty + rezerwacji
                    Data_Rozpoczecia = result.rezerwacja.Data_Rozpoczecia,
                    Data_Zakonczenia = result.rezerwacja.Data_Zakonczenia,
                    // Pobranie opisu wybranego ubezpieczenia z sesji
                    WybraneUbezpieczenie = HttpContext.Session.GetString($"InsuranceDescription_{result.rezerwacja.Id}")
                })
                .ToList();

            return View(rezerwacje);
        }

        public IActionResult AddInsurance(int id)
        {
            var reservation = _context.Rezerwacja.FirstOrDefault(r => r.Id == id);
            if (reservation == null) return NotFound();

            // Load insurance options from the database
            var insuranceOptions = _context.Ubezpieczenie.ToList();
            ViewBag.ReservationId = id;
            return View(insuranceOptions);
        }

        [HttpPost]
        public IActionResult AddInsurance(int id, int selectedInsuranceId)
        {
            var reservation = _context.Rezerwacja.FirstOrDefault(r => r.Id == id);
            if (reservation == null) return NotFound();

            // Pobranie wybranego ubezpieczenia
            var insuranceTemplate = _context.Ubezpieczenie.FirstOrDefault(i => i.Id == selectedInsuranceId);
            if (insuranceTemplate != null)
            {
                // Pobranie oferty powiązanej z rezerwacją
                var offer = _context.Samochod
                    .Where(car => car.Id == reservation.Samochod_Id)
                    .Select(car => car.Oferta)
                    .FirstOrDefault();

                if (offer != null)
                {
                    // Obliczenie nowej ceny z ubezpieczeniem
                    reservation.Cena += insuranceTemplate.Wartosc;

                    // Zapisanie zmiany w bazie danych
                    _context.SaveChanges();

                    // Pobieranie obecnych ubezpieczeń z sesji
                    var existingInsurance = HttpContext.Session.GetString($"InsuranceDescription_{id}");
                    if (!string.IsNullOrEmpty(existingInsurance))
                    {
                        // Dodanie nowego ubezpieczenia do istniejącego opisu (oddzielając przecinkiem)
                        existingInsurance += $", {insuranceTemplate.Typ}";
                    }
                    else
                    {
                        // Jeśli brak ubezpieczeń, zapisujemy pierwsze
                        existingInsurance = insuranceTemplate.Typ;
                    }

                    // Zapisanie nowego ciągu ubezpieczeń do sesji
                    HttpContext.Session.SetString($"InsuranceDescription_{id}", existingInsurance);
                }
            }

            // Redirect do Index po zapisaniu ubezpieczenia
            return RedirectToAction("Index");
        }




        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var reservation = _context.Rezerwacja.FirstOrDefault(r => r.Id == id);
            if (reservation != null)
            {
                _context.Rezerwacja.Remove(reservation);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
