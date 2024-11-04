﻿using Microsoft.AspNetCore.Mvc;
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
                    Cena = (decimal)result.offer.Cena, // Initial offer price
                    Data_Rozpoczecia = result.rezerwacja.Data_Rozpoczecia,
                    Data_Zakonczenia = result.rezerwacja.Data_Zakonczenia
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

            var insurance = _context.Ubezpieczenie.FirstOrDefault(i => i.Id == selectedInsuranceId);
            if (insurance != null)
            {
                // Retrieve the car's offer price
                var offerPrice = _context.Samochod
                    .Where(car => car.Id == reservation.Samochod_Id)
                    .Select(car => car.Oferta.Cena)
                    .FirstOrDefault();

                // Calculate the new reservation price with insurance
                var newCena = (decimal)(offerPrice + insurance.Wartosc);

                // Link the insurance to the reservation
                insurance.Rezerwacja_Id = id;
                _context.SaveChanges();
            }

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