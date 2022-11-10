using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SemestralnaPraca.Models;
using System.Diagnostics;

namespace SemestralnaPraca.Controllers
{
    public class HomeController : Controller
    {
        // TODO doplnenie tabulky a zalozky s navstevovanostou
        // TODO AJAX
        // TODO novy page pre poziadavky RequestManagement

        private readonly IHttpContextAccessor context;
        string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"];

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            context = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult FormSubmit()
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(SessionVariables.Mail)))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult FormSubmit(int category, string description)
        {
            string user = context.HttpContext.Session.GetString(SessionVariables.Mail);

            if (!string.IsNullOrEmpty(user))
            {
                if (DataResolver.InsertRequest(user, category, description))
                {
                    TempData["SuccessReply"] = ("požiadavka bola úspešne zaregistrovaná");
                }
                else
                {
                    TempData["ErrorReply"] = ("nepodarilo sa vytvoriť požiadavku");
                }

                return RedirectToAction("RequestManagement", "Home");
            }
            else
            { 
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Register()
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(SessionVariables.Mail)))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Register(string mail, string name, string surname, string password, string confirmation, string phone)
        {
            int role = 1;
            ViewBag.Mail = mail;
            ViewBag.Name = name;
            ViewBag.Surname = surname;
            ViewBag.Phone = phone;

            UserModel user = new UserModel();
            user.MAIL = mail;
            user.NAME = name;
            user.SURNAME= surname;
            user.PHONE= phone;
            user.PASSWORD = password;
            user.ROLE= role;

            if (DataResolver.InsertUser(user))
            {
                ViewBag.Reply += "nepodarilo sa dokončiť rezerváciu, účet so zadaným mailom už existuje";
                return View();
            }
            else
            {
                LoginAction(mail, DatabaseTranslator.Access[role]);
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Login()
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(SessionVariables.Mail)))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Login(string mail, string password)
        {
            ViewBag.Mail = mail;

            UserModel user = DataResolver.GetUser(mail);

            if (user != null)
            {
                if (DataResolver.Hash(password) == user.PASSWORD)
                {
                    LoginAction(user.MAIL, DatabaseTranslator.Access[user.ROLE]);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Reply += "nesprávne zadané heslo";
                }
            }
            else
            {
                ViewBag.Reply += "daný účet neexistuje";
            }

            return View();
        }

        public IActionResult UserManagement()
        {
            if (DatabaseTranslator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(SessionVariables.Role)).Key >= 2)
            {
                ViewBag.SuccessReply = TempData["SuccessReply"];
                ViewBag.ErrorReply = TempData["ErrorReply"];
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult AccountDetails() 
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(SessionVariables.Mail)))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.User = TempData["User"];
                return View();
            }
        }

        [HttpPost]
        public IActionResult AccountDetails(string mail, string name, string surname, string phone, string password, int role)
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(SessionVariables.Mail)))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                UserModel user = new UserModel();

                user.MAIL = mail;
                user.NAME = name;
                user.SURNAME = surname;
                user.PHONE = phone;
                user.PASSWORD = password;
                user.ROLE = (role + 1);

                if (DataResolver.UpdateUser(user))
                {
                    ViewBag.SuccessReply = "údaje boli uložené";
                }
                else
                {
                    ViewBag.ErrorReply = "údaje sa nepodarilo uložiť";
                }

                ViewBag.User = mail;

                return View();
            }
        }

        public IActionResult EditUser(string mail)
        {
            if (DatabaseTranslator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(SessionVariables.Role)).Key >= 2)
            {
                TempData["User"] = mail;
                return RedirectToAction("AccountDetails", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult DeleteUser(string mail)
        {
            if (DatabaseTranslator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(SessionVariables.Role)).Key >= 2)
            {
                if (DataResolver.DeleteUser(mail))
                {
                    TempData["SuccessReply"] = ("používateľ " + mail + " bol odstránený");
                }
                else
                {
                    TempData["ErrorReply"] = ("používatela " + mail + " sa nepodarilo odstrániť");
                }

;               return RedirectToAction("UserManagement", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            LogoutAction();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Wedding()
        {
            return View("~/Views/Gallery/Wedding.cshtml");
        }

        public IActionResult Event()
        {
            return View("~/Views/Gallery/Event.cshtml");
        }

        public IActionResult Car()
        {
            return View("~/Views/Gallery/Car.cshtml");
        }

        public IActionResult Nature()
        {
            return View("~/Views/Gallery/Nature.cshtml");
        }

        public IActionResult Other()
        {
            return View("~/Views/Gallery/Other.cshtml");
        }

        private void LoginAction(string mail, string role)
        {
            context.HttpContext.Session.SetString(SessionVariables.Mail, mail);
            context.HttpContext.Session.SetString(SessionVariables.Role, role);
        }

        private void LogoutAction()
        {
            context.HttpContext.Session.SetString(SessionVariables.Mail, string.Empty);
            context.HttpContext.Session.SetString(SessionVariables.Role, string.Empty);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}