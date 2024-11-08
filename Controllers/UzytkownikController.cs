using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Carente.Models;

public class UzytkownikController : Controller
{
    private readonly CarenteContext _context;
    private readonly IPasswordHasher<Uzytkownik> _passwordHasher; // Dodajemy haszowanie haseł

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

    // Edycja danych użytkownika (w tym weryfikacja starego hasła)
    [HttpPost]
    public async Task<IActionResult> Edit(Uzytkownik updatedUser, string oldPassword)
    {
        if (!ModelState.IsValid)
        {
            return View("user", updatedUser); // Jeśli dane są niepoprawne, wróć do formularza
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        var existingUser = await _context.Uzytkownik.FindAsync(userId);

        if (existingUser == null)
        {
            return NotFound();
        }

        // Weryfikacja starego hasła
        var result = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.haslo, oldPassword);
        if (result != PasswordVerificationResult.Success)
        {
            ModelState.AddModelError("", "Potwierdzenie hasłem jest nieprawidłowe.");
            return View("user", updatedUser); // Jeśli stare hasło jest błędne, wróć do formularza
        }

        // Edytowanie tylko tych pól, które zostały zmienione
        existingUser.imie = updatedUser.imie ?? existingUser.imie;
        existingUser.nazwisko = updatedUser.nazwisko ?? existingUser.nazwisko;
        existingUser.email = updatedUser.email ?? existingUser.email;
        existingUser.tel = updatedUser.tel ?? existingUser.tel;

        // Jeśli nowe hasło zostało wprowadzone, zaktualizuj hasło
        if (!string.IsNullOrEmpty(updatedUser.haslo))
        {
            existingUser.haslo = _passwordHasher.HashPassword(existingUser, updatedUser.haslo);
        }
        else if (!string.IsNullOrEmpty(oldPassword)) // Jeśli nowe hasło nie zostało podane, ale stary hasło jest wprowadzony, potwierdź zmiany
        {
            existingUser.haslo = _passwordHasher.HashPassword(existingUser, oldPassword);
        }

        await _context.SaveChangesAsync();  // Zapisz zmiany w bazie danych
        return RedirectToAction("UserProfile"); // Przekierowanie do profilu użytkownika
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
