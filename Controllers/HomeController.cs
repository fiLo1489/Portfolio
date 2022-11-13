using Microsoft.AspNetCore.Mvc;
using SemestralnaPraca.Models;
using System.Data;
using System.Diagnostics;

namespace SemestralnaPraca.Controllers
{
    public class HomeController : Controller
    {
        // TODO odoslanie upravenej ziadosti
        // TODO validacny skript pre úpravu formularu
        // TODO uprava fotky v o mne v mobilnom rozhrani
        // TODO doplnenie spravy fotiek
        // TODO AJAX
        // TODO doplnenie tabulky a zalozky s navstevovanostou
        // TODO validacia HTML
        // TODO skript na cistenie uloziska

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
            if (!string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult FormSubmit(int category, string description, string date)
        {
            string user = context.HttpContext.Session.GetString(Variables.Mail);

            if (!string.IsNullOrEmpty(user))
            {
                RequestModel request = new RequestModel();

                request.CATEGORY = Translator.Categories.ElementAt(category).Key;
                request.DESCRIPTION = description;
                request.SCHEDULED = date;
                request.USER = user;

                bool? result = RequestController.InsertRequest(request) == true;

                if (result == true)
                {
                    ViewBag.SuccessReply = ("požiadavka bola úspešne zaregistrovaná");
                }
                else if (result == false)
                {
                    ViewBag.ErrorReply = ("požiadavka v daný deň už existuje");
                }
                else
                {
                    ViewBag.ErrorReply = ("nepodarilo sa vytvoriť požiadavku");
                }

                return View();
            }
            else
            { 
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult FormSubmit(int id, string scheduled, string description, int status, string result)
        {
            //string user = context.HttpContext.Session.GetString(Variables.Mail);

            //if (!string.IsNullOrEmpty(user))
            //{
            //    RequestModel request = new RequestModel();

            //    request.CATEGORY = Translator.Categories.ElementAt(category).Key;
            //    request.DESCRIPTION = description;
            //    request.SCHEDULED = date;
            //    request.USER = user;

            //    bool? result = RequestController.InsertRequest(request) == true;

            //    if (result == true)
            //    {
            //        ViewBag.SuccessReply = ("požiadavka bola úspešne zaregistrovaná");
            //    }
            //    else if (result == false)
            //    {
            //        ViewBag.ErrorReply = ("požiadavka v daný deň už existuje");
            //    }
            //    else
            //    {
            //        ViewBag.ErrorReply = ("nepodarilo sa vytvoriť požiadavku");
            //    }

            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return View();
        }

        public IActionResult Register()
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
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

            bool? result = UserController.InsertUser(user);

            if (result == true)
            {
                LoginAction(mail, Translator.Access[role]);
                return RedirectToAction("Index", "Home");
            }
            else if (result == false)
            {
                ViewBag.Reply += "účet so zadaným mailom už existuje";
                return View();
            }
            else
            {
                ViewBag.Reply += "nepodarilo sa dokončiť rezerváciu";
                return View();
            }
        }

        public IActionResult Login()
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
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

            UserModel user = UserController.GetUser(mail);

            if (user != null)
            {
                if (UserController.GetPassword(password) == user.PASSWORD)
                {
                    LoginAction(user.MAIL, Translator.Access[user.ROLE]);

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
            if (Translator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(Variables.Role)).Key >= 2)
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

        public IActionResult RequestManagement()
        {
            if (!string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
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
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
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
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
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

                if (UserController.UpdateUser(user))
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
            if (Translator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(Variables.Role)).Key >= 2)
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
            if (Translator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(Variables.Role)).Key >= 2)
            {
                if (UserController.DeleteUser(mail))
                {
                    TempData["SuccessReply"] = ("používateľ " + mail + " bol odstránený");
                }
                else
                {
                    TempData["ErrorReply"] = ("používatela " + mail + " sa nepodarilo odstrániť");
                }

                return RedirectToAction("UserManagement", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EditRequest(string id)
        {
            if (!string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
            {
                TempData["Id"] = id;
                return RedirectToAction("RequestEdit", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult DeleteRequest(string id)
        {
            if (!string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
            {
                bool admin = (Translator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(Variables.Role)).Key >= 2) ? true : false;
                bool? result = RequestController.DeleteRequest(id, admin);

                if (result == true)
                {
                    TempData["SuccessReply"] = ("požiadavka číslo " + id + " bola odstránená");
                }
                else if (result == false)
                {
                    TempData["ErrorReply"] = ("požiadavku číslo " + id + " nemožno odstrániť pretože už nie je čakajúca");
                }
                else
                {
                    TempData["ErrorReply"] = ("požiadavku číslo " + id + " sa nepodarilo odstrániť");
                }

                return RedirectToAction("RequestManagement", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult RequestEdit()
        {
            ViewBag.Id = TempData["Id"];
            return View();

            // TODO pristup
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
            context.HttpContext.Session.SetString(Variables.Mail, mail);
            context.HttpContext.Session.SetString(Variables.Role, role);
        }

        private void LogoutAction()
        {
            context.HttpContext.Session.SetString(Variables.Mail, string.Empty);
            context.HttpContext.Session.SetString(Variables.Role, string.Empty);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}