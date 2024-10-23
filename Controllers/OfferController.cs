using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class OfferController : Controller
{
    private readonly CarenteContext _context;

    public OfferController(CarenteContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("~/Views/Shared/offer_form.cshtml"); // Zwróć widok z folderu Shared
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Offer oferta)
    {
        if (ModelState.IsValid)
        {
            _context.Oferta.Add(oferta);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index"); // lub inna akcja po dodaniu oferty
        }
        return View("~/Views/Shared/offer_form.cshtml", oferta); // Zwróć formularz z błędami
    }
}
