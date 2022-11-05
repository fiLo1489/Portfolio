using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string login)
        {
            UserModel user = new UserModel();

            user.LOGIN = login;

            context.HttpContext.Session.SetString(SessionVariables.UserName, login);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            context.HttpContext.Session.SetString(SessionVariables.UserName, string.Empty);

            return RedirectToAction("Index", "Home");
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