using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sistema_RRHH_Nexora.Models;

namespace Sistema_RRHH_Nexora.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Home/Error")]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }

            // Manejar otros códigos de estado si es necesario
            return View("Error");
        }
    }
}
