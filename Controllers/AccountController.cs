using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly CarenteContext _context;
    private readonly IPasswordHasher<Uzytkownik> _passwordHasher; // Dodajemy haszowanie haseł

    public AccountController(CarenteContext context, IPasswordHasher<Uzytkownik> passwordHasher)
    {
        _context = context; // Wstrzyknięcie kontekstu bazy danych
        _passwordHasher = passwordHasher; // Wstrzyknięcie haszowania haseł
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(); // Wyświetla formularz logowania
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Uzytkownik
                .Where(u => u.email == model.email)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                // Weryfikacja hasła
                var result = _passwordHasher.VerifyHashedPassword(user, user.haslo, model.haslo);
                if (result == PasswordVerificationResult.Success)
                {
                    // Ustaw sesję
                    HttpContext.Session.SetInt32("UserId", user.id);
                    HttpContext.Session.SetInt32("typ", user.typ ? 1 : 0);
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError("", "Nieprawidłowe hasło.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Użytkownik o podanym adresie email nie istnieje.");
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(); // Wyświetla formularz rejestracji
    }

    [HttpPost]
    public async Task<IActionResult> Register(Register model)
    {
        if (ModelState.IsValid)
        {
            // Tworzenie nowego użytkownika
            var uzytkownik = new Uzytkownik
            {
                imie = model.imie,
                nazwisko = model.nazwisko,
                email = model.email,
                // Hashowanie hasła przed zapisaniem
                haslo = _passwordHasher.HashPassword(null, model.haslo),
                tel = model.tel,
                typ = false
            };

            _context.Uzytkownik.Add(uzytkownik);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login"); // Przekierowanie do strony logowania
        }

        return View(model); // Jeśli dane są nieprawidłowe, powraca do formularza z błędami
    }

    [HttpPost]
    public IActionResult Logout()
    {
        // Usunięcie danych sesji
        HttpContext.Session.Remove("UserId"); // Usunięcie ID użytkownika z sesji
        return RedirectToAction("Index", "Home"); // Przekierowanie na stronę główną
    }
}
