using Carente.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Carente.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult User()
        {
            return View();


        }

        public IActionResult Details(int id)
        {
            var model = new ImageDetailsViewModel
            {
                Id = id,
                Title = $"Image {id}",
                Description = $"Details of image {id}",
                ImageUrl = $"/Images/1.png"
            };

            return View(model);
        }
    }
}
