using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NuGet.Protocol.Plugins;
using SemestralnaPraca.Models;
using System.Diagnostics;
using System.Drawing;
using System.Net.Mail;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            return View();
        }

        [HttpPost]
        public IActionResult Register(string mail, string name, string surname, string password, string confirmation, string phone)
        {
            string errorMessage = string.Empty;
            ViewBag.Mail = mail;
            ViewBag.Name = name;
            ViewBag.Surname = surname;
            ViewBag.Phone = phone;

            errorMessage += DataHandler.CheckMail(mail);
            errorMessage += DataHandler.CheckPhone(phone);

            if (!password.Equals(confirmation))
            {
                errorMessage += "heslá sa nezhodujú, ";
            }
            
            errorMessage += DataHandler.CheckPassword(password);

            if (string.IsNullOrEmpty(errorMessage))
            {
                string hashPassword = DataHandler.Hash(password);
                string userRole = "user";

                string query = "insert into CREDENTIALS values " +
                    "('" + mail + "', '" + hashPassword + "', '" + userRole + "', '" + name + "', '" + surname + "', '" + phone + "')";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                LoginAction(mail, userRole, name, surname, phone);

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
            return View();
        }

        [HttpPost]
        public IActionResult Login(string mail, string password)
        {
            string errorMessage = string.Empty;
            ViewBag.Mail = mail;

            errorMessage += DataHandler.CheckMail(mail);
            errorMessage += DataHandler.CheckPassword(password);

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

                                if ((string)reader[1] == DataHandler.Hash(password))
                                {
                                    LoginAction((string)reader[0], (string)reader[2], (string)reader[3], (string)reader[4], (string)reader[5]);

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