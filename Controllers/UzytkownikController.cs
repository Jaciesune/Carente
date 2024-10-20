using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class UzytkownikController : Controller
{
    private readonly CarenteContext _context;

    public UzytkownikController(CarenteContext context)
    {
        _context = context; // Wstrzyknięcie kontekstu bazy danych
    }

    // Metoda do wyświetlania profilu obecnie zalogowanego użytkownika
    [HttpGet]
    public async Task<IActionResult> UserProfile()
    {
        var userIdString = HttpContext.Session.GetString("UserId"); // Pobierz ID użytkownika z sesji

        if (string.IsNullOrEmpty(userIdString))
        {
            return RedirectToAction("Login", "Account"); // Przekierowanie do logowania, jeśli ID nie istnieje
        }

        if (int.TryParse(userIdString, out int userId))
        {
            var uzytkownik = await _context.Uzytkownik.FindAsync(userId); // Znajdź użytkownika na podstawie ID

            if (uzytkownik == null)
            {
                return NotFound(); // Zwróć 404, jeśli użytkownik nie istnieje
            }

            return View("user", uzytkownik); // Zwróć widok z danymi pojedynczego użytkownika
        }

        return RedirectToAction("Login", "Account"); // Przekierowanie do logowania, jeśli nie uda się sparsować ID
    }

    // Metoda do wyświetlania wszystkich użytkowników (opcja, jeśli potrzebna)
    public async Task<IActionResult> Index()
    {
        var uzytkownicy = await _context.Uzytkownik.ToListAsync(); // Pobierz wszystkich użytkowników
        return View("user", uzytkownicy); // Zwróć widok Index z listą użytkowników
    }
}
