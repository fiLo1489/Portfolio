﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string mail, string password)
        {
            string errorMessage = string.Empty;
            ViewBag.Mail = mail;

            if (string.IsNullOrWhiteSpace(mail))
            {
                errorMessage += "nebol zadaný mail, ";
            }
            else
            {
                if (!mail.Trim().Contains('@') || !mail.Trim().Contains('.'))
                {
                    errorMessage += "zadaný mail má nesprávny tvar, ";
                }

                if (mail.Contains("select ") || mail.Contains("delete ") || mail.Contains("alter ") || mail.Contains("update "))
                {
                    errorMessage += "zadaný mail obashuje nepovolené kľúčové slová, ";
                }
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage += "nebolo zadané heslo, ";
            }
            else
            {
                if (password.Contains("select ") || password.Contains("delete ") || password.Contains("alter ") || password.Contains("update "))
                {
                    errorMessage += "zadané heslo obashuje nepovolené kľúčové slová, ";
                }
            }

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                string query = "select * from CREDENTIALS where MAIL='" + mail + "'";
                string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"];

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

                                if ((string)reader[1] == password)
                                {
                                    context.HttpContext.Session.SetString(SessionVariables.Mail, (string)reader[0]);
                                    context.HttpContext.Session.SetString(SessionVariables.Role, (string)reader[2]);
                                    context.HttpContext.Session.SetString(SessionVariables.Name, (string)reader[3]);
                                    context.HttpContext.Session.SetString(SessionVariables.Surname, (string)reader[4]);
                                    context.HttpContext.Session.SetString(SessionVariables.Phone, (string)reader[5]);

                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    errorMessage += "nesprávne zadané heslo";

                                    ViewBag.ErrorMessage = errorMessage;
                                    return View();
                                }
                            }
                            else
                            {
                                errorMessage += "daný účet neexistuje";

                                ViewBag.ErrorMessage = errorMessage;
                                return View();
                            }
                        }
                    }
                }
            }
            else
            {
                errorMessage = errorMessage.Remove(errorMessage.Length - 2);

                ViewBag.ErrorMessage = errorMessage;
                return View();
            }
        }

        public IActionResult Logout()
        {
            context.HttpContext.Session.SetString(SessionVariables.Mail, string.Empty);
            context.HttpContext.Session.SetString(SessionVariables.Name, string.Empty);
            context.HttpContext.Session.SetString(SessionVariables.Surname, string.Empty);
            context.HttpContext.Session.SetString(SessionVariables.Phone, string.Empty);
            context.HttpContext.Session.SetString(SessionVariables.Role, string.Empty);

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}