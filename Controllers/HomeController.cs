using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NutriTrackAI.Models;

namespace NutriTrackAI.Controllers
{
    //Handles the home, privacy, and error page
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //shows home page
        public IActionResult Index()
        {
            return View();
        }

        //Shows privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        //Shows an error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
