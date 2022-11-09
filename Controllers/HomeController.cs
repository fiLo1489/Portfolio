using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace SemestralnaPraca.Controllers
{
    public class HomeController : Controller
    {
        // TODO doplnenie tabulky s navstevovanostou
        // TODO ajax
        // TODO suhlas s cookies
        // TODO overenie pred odoslanim ziadosti
        // TODO rozdelenie do viacerych controllerov
        // TODO v uzivateloch zobrazovat mensiu uroven
        
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
                string formQuery = "insert into REQUESTS values " +
                    "('" + user + "', '" + DatabaseTranslator.Categories.ElementAt(category).Key + "', '1', '" + description + "')";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(formQuery, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

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
            int exists = 0;
            int role = 1;

            ViewBag.Mail = mail;
            ViewBag.Name = name;
            ViewBag.Surname = surname;
            ViewBag.Phone = phone;

            string existsQuery = "select count(*) from CREDENTIALS where MAIL='" + mail + "'";
            string registerQuery = "insert into CREDENTIALS values " +
                    "('" + mail + "', '" + DataResolver.Hash(password) + "', '" + name + "', '" + surname + "', '" + phone + "', '" + role + "')";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(existsQuery, connection))
                {
                    exists = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (exists == 0)
                {
                    using (SqlCommand cmd = new SqlCommand(registerQuery, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    LoginAction(mail, DatabaseTranslator.Access[role], name, surname, phone);
                }
                else
                {
                    reply += "používateľ so zadaným mailom už existuje, ";
                }
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

            if (string.IsNullOrWhiteSpace(reply))
            {
                UserModel account = DataResolver.GetAccount(mail);

                if (account != null)
                { 
                    if (DataResolver.Hash(password) == account.PASSWORD)
                    {
                        LoginAction(account.MAIL, DatabaseTranslator.Access[account.ROLE], account.NAME, account.SURNAME, account.PHONE);

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
            else
            {
                reply = reply.Remove(reply.Length - 2);

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
                if (!string.IsNullOrEmpty(mail))
                {
                    string query = "delete from CREDENTIALS where MAIL='" + mail + "'";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    TempData["Reply"] = ("používateľ " + mail + " bol úspešne odstránený");
                }
                else
                {
                    TempData["Reply"] = ("nepodarilo sa odstrániť používateľa " + mail);
                }

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

        private void LoginAction(string mail, string role, string name, string surname, string phone)
        {
            context.HttpContext.Session.SetString(SessionVariables.Mail, mail);
            context.HttpContext.Session.SetString(SessionVariables.Role, role);
            context.HttpContext.Session.SetString(SessionVariables.Name, name);
            context.HttpContext.Session.SetString(SessionVariables.Surname, surname);
            context.HttpContext.Session.SetString(SessionVariables.Phone, phone);
        }

        private void LogoutAction()
        {
            context.HttpContext.Session.SetString(SessionVariables.Mail, string.Empty);
            context.HttpContext.Session.SetString(SessionVariables.Name, string.Empty);
            context.HttpContext.Session.SetString(SessionVariables.Surname, string.Empty);
            context.HttpContext.Session.SetString(SessionVariables.Phone, string.Empty);
            context.HttpContext.Session.SetString(SessionVariables.Role, string.Empty);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}