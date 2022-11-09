using Microsoft.AspNetCore.Mvc;
using SemestralnaPraca.Models;
using System.Diagnostics;

namespace SemestralnaPraca.Controllers
{
    public class HomeController : Controller
    {
        // TODO doplnenie tabulky a zalozky s navstevovanostou
        // TODO ajax
        // TODO hlasky pre operacie do okien
        
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
            return View();
        }

        [HttpPost]
        public IActionResult FormSubmit(int category, string description)
        {
            string user = context.HttpContext.Session.GetString(SessionVariables.Mail);

            if (!string.IsNullOrEmpty(user))
            {
                DataResolver.InsertRequest(user, category, description);

                return RedirectToAction("Index", "Reuqests");
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
            string reply = string.Empty;
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
                reply += "používateľ so zadaným mailom už existuje, ";
            }
            else
            {
                LoginAction(mail, DatabaseTranslator.Access[role]);
            }

            if (reply.Equals(string.Empty))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                reply = reply.Remove(reply.Length - 2);
                ViewBag.Reply = reply;
                return View();
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
            string reply = string.Empty;
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
                    reply += "nesprávne zadané heslo";

                    ViewBag.Reply = reply;
                    return View();
                }
            }
            else
            {
                reply += "daný účet neexistuje";

                ViewBag.Reply = reply;
                return View();
            }
        }

        public IActionResult UserManagement()
        {
            if (DatabaseTranslator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(SessionVariables.Role)).Key >= 2)
            {
                ViewBag.Reply = TempData["Reply"];
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
                user.ROLE = role;

                DataResolver.UpdateUser(user);

                ViewBag.User = mail;
                ViewBag.Reply = "údaje boli uložené";
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
                DataResolver.DeleteUser(mail);

                TempData["Reply"] = ("používateľ " + mail + " bol odstránený");

                return RedirectToAction("UserManagement", "Home");
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