﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Carente.Models;

public class UzytkownikController : Controller
{
    private readonly CarenteContext _context;
    private readonly IPasswordHasher<Uzytkownik> _passwordHasher; // Obsługa haszowania haseł

    public UzytkownikController(CarenteContext context, IPasswordHasher<Uzytkownik> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    // Wyświetlenie profilu użytkownika (w zależności od typu użytkownika)
    [HttpGet]
    public async Task<IActionResult> UserProfile()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("typ");

        if (userId == null || userType == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (userType == 1) // Widok admina dla wszystkich użytkowników
        {
            var uzytkownicy = await _context.Uzytkownik.ToListAsync();
            return View("UsersList", uzytkownicy); // Widok dla admina
        }
        else // Widok zwykłego użytkownika - tylko jego profil
        {
            var uzytkownik = await _context.Uzytkownik.FindAsync(userId);

            if (uzytkownik == null)
            {
                return NotFound();
            }

            return View("user", uzytkownik); // Widok dla zwykłego użytkownika
        }
    }

    // Metoda GET dla edycji użytkownika przez niego samego
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var uzytkownik = await _context.Uzytkownik.FindAsync(userId);
        if (uzytkownik == null)
        {
            return NotFound();
        }

        return View("user", uzytkownik); // Widok edycji profilu użytkownika
    }

    // Edycja danych użytkownika przez niego samego (z weryfikacją starego hasła)
    [HttpPost]
    public async Task<IActionResult> Edit(Uzytkownik updatedUser, string oldPassword, string confirmPassword)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var existingUser = await _context.Uzytkownik.FindAsync(userId);
        if (existingUser == null)
        {
            return NotFound();
        }

        // Weryfikacja starego hasła
        var passwordVerification = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.haslo, oldPassword);
        if (passwordVerification != PasswordVerificationResult.Success)
        {
            ModelState.AddModelError("", "Potwierdzenie hasłem jest nieprawidłowe.");
            return View("user", existingUser);
        }

        // Aktualizacja danych tylko jeśli zmienione
        if (updatedUser.imie != existingUser.imie)
            existingUser.imie = updatedUser.imie;
        if (updatedUser.nazwisko != existingUser.nazwisko)
            existingUser.nazwisko = updatedUser.nazwisko;
        if (updatedUser.email != existingUser.email)
            existingUser.email = updatedUser.email;
        if (updatedUser.tel != existingUser.tel)
            existingUser.tel = updatedUser.tel;

        // Aktualizacja hasła, jeśli nowe hasło zostało podane i potwierdzenie jest zgodne
        if (!string.IsNullOrEmpty(updatedUser.haslo))
        {
            if (updatedUser.haslo == confirmPassword)
            {
                existingUser.haslo = _passwordHasher.HashPassword(existingUser, updatedUser.haslo);
            }
            else
            {
                ModelState.AddModelError("", "Nowe hasło i jego potwierdzenie nie są zgodne.");
                return View("user", existingUser);
            }
        }

        await _context.SaveChangesAsync();

        // Odświeżenie widoku profilu z aktualnymi danymi
        return View("user", existingUser);
    }

    // Metoda GET dla edycji danych użytkownika przez admina
    [HttpGet]
    public async Task<IActionResult> EditUserByAdmin(int id)
    {
        var adminId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("typ");

        if (userType != 1)
        {
            return Unauthorized(); // Tylko admin może edytować innych użytkowników
        }

        var user = await _context.Uzytkownik.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View("EditUserByAdmin", user);
    }

    // Edycja danych użytkownika przez admina (z weryfikacją hasła admina)
    [HttpPost]
    public async Task<IActionResult> EditUserByAdmin(int id, Uzytkownik updatedUser, string adminPassword)
    {
        var adminId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("typ");

        if (userType != 1)
        {
            return Unauthorized();
        }

        var admin = await _context.Uzytkownik.FindAsync(adminId);
        var userToEdit = await _context.Uzytkownik.FindAsync(id);

        if (userToEdit == null || admin == null)
        {
            return NotFound();
        }

        // Weryfikacja hasła admina
        var passwordVerification = _passwordHasher.VerifyHashedPassword(admin, admin.haslo, adminPassword);
        if (passwordVerification != PasswordVerificationResult.Success)
        {
            ModelState.AddModelError("", "Nieprawidłowe hasło admina.");
            return View("EditUserByAdmin", userToEdit);
        }

        // Aktualizacja danych użytkownika
        userToEdit.imie = updatedUser.imie;
        userToEdit.nazwisko = updatedUser.nazwisko;
        userToEdit.email = updatedUser.email;
        userToEdit.tel = updatedUser.tel;

        if (!string.IsNullOrEmpty(updatedUser.haslo))
        {
            userToEdit.haslo = _passwordHasher.HashPassword(userToEdit, updatedUser.haslo);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("UsersList");
    }

    // Usuwanie użytkownika
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Uzytkownik.FindAsync(id);
        if (user != null)
        {
            _context.Uzytkownik.Remove(user);
            await _context.SaveChangesAsync();
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("typ");

        // Jeśli admin, przekierowanie do listy użytkowników, w przeciwnym razie do profilu
        if (userType == 1)
        {
            return RedirectToAction("UsersList"); // Dla admina - powrót do listy użytkowników
        }

        return RedirectToAction("UserProfile"); // Dla zwykłego użytkownika - powrót do jego profilu
    }

    // Metoda wyświetlająca listę użytkowników (tylko dla admina)
    public async Task<IActionResult> UsersList()
    {
        var uzytkownicy = await _context.Uzytkownik.ToListAsync();
        return View(uzytkownicy);
    }
}
