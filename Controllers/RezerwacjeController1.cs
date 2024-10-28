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
                    Id = result.rezerwacja.Id, // Add Id to identify each reservation
                    Marka = result.car.Marka,
                    Model = result.car.Model,
                    Cena = (decimal)result.offer.Cena,
                    Data_Rozpoczecia = result.rezerwacja.Data_Rozpoczecia,
                    Data_Zakonczenia = result.rezerwacja.Data_Zakonczenia
                })
                .ToList();

            return View(rezerwacje);
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
