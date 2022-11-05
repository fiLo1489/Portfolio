using Microsoft.AspNetCore.Mvc;
using SemestralnaPraca.Models;
using System.Diagnostics;

namespace SemestralnaPraca.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor context;
        
        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            context = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Form()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Wedding()
        {
            return View();
        }

        public IActionResult Event()
        {
            return View();
        }

        public IActionResult Car()
        {
            return View();
        }

        public IActionResult Nature()
        {
            return View();
        }

        public IActionResult Other()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}