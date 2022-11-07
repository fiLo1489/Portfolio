using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;
using System.Diagnostics;

namespace SemestralnaPraca.Controllers
{
    public class HomeController : Controller
    {
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

        public IActionResult Form()
        {
            return View();
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
            string errorMessage = string.Empty;
            ViewBag.Mail = mail;
            ViewBag.Name = name;
            ViewBag.Surname = surname;
            ViewBag.Phone = phone;
            
            // TODO kontrola, ci uz uzivatel neexistuje
            // TODO doplnenie tabuľky

            if (string.IsNullOrEmpty(errorMessage))
            {
                string credentialsQuery = "insert into CREDENTIALS values " +
                    "('" + mail + "', '" + DataResolver.Hash(password) + "', '" + name + "', '" + surname + "', '" + phone + "', '1')";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(credentialsQuery, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                LoginAction(mail, 2, name, surname, phone);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                errorMessage = errorMessage.Remove(errorMessage.Length - 2);

                ViewBag.Reply = errorMessage;
                return View();
            }

            return View();
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
            string errorMessage = string.Empty;
            ViewBag.Mail = mail;

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                string query = "select * from CREDENTIALS where MAIL='" + mail + "'";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();

                                if (reader[1].ToString() == DataResolver.Hash(password))
                                {
                                    LoginAction(reader[0].ToString(), int.Parse(reader[5].ToString()), reader[2].ToString(), reader[3].ToString(), reader[4].ToString());

                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    errorMessage += "nesprávne zadané heslo";

                                    ViewBag.Reply = errorMessage;
                                    return View();
                                }
                            }
                            else
                            {
                                errorMessage += "daný účet neexistuje";

                                ViewBag.Reply = errorMessage;
                                return View();
                            }
                        }
                    }
                }
            }
            else
            {
                errorMessage = errorMessage.Remove(errorMessage.Length - 2);

                ViewBag.Reply = errorMessage;
                return View();
            }
        }

        public IActionResult Logout()
        {
            LogoutAction();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Account()
        {
            if (!string.IsNullOrEmpty(context.HttpContext.Session.GetString(SessionVariables.Mail)))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
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

        private void LoginAction(string mail, int role, string name, string surname, string phone)
        {
            context.HttpContext.Session.SetString(SessionVariables.Mail, mail);
            context.HttpContext.Session.SetInt32(SessionVariables.Role, role);
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
            context.HttpContext.Session.SetInt32(SessionVariables.Role, 0);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}