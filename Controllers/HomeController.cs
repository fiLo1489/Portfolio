using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemestralnaPraca.Models;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;

namespace SemestralnaPraca.Controllers
{
    public class HomeController : Controller
    {
        // TODO upratanie home controllera
        // TODO AJAX pre zobrazovanie statistiky
        // TODO validacia HTML

        private readonly IHttpContextAccessor context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment enviroment;
        string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"];

        public HomeController(IHttpContextAccessor httpContextAccessor, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            context = httpContextAccessor;
            enviroment = hostingEnvironment;
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
        public List<object> Statistics(string date)
        { 
            List<object> data = new List<object>();

            List<string> labels = Translator.Categories.Values.ToList();
            data.Add(labels);

            List<int> values = StatisticsController.GetStatistics(date);
            data.Add(values);

            return data;
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

                bool? output = RequestController.InsertRequest(request);

                if (output == true)
                {
                    ViewBag.SuccessReply = ("požiadavka bola úspešne zaregistrovaná");
                }
                else if (output == false)
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
        public IActionResult RequestEdit(int id, string scheduled, string description, int status, string result)
        {
            RequestModel request = new RequestModel();

            request.ID = id;
            request.SCHEDULED = scheduled;
            request.DESCRIPTION = description;
            request.STATUS = Translator.Status.ElementAt(status).Value;
            request.RESULT = result;

            bool? output = RequestController.UpdateRequest(request, (Translator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(Variables.Role)).Key >= 2));

            if (output == true)
            {
                ViewBag.SuccessReply = ("požiadavka bola úspešne aktualizovaná");
            }
            else if (output == false)
            {
                ViewBag.ErrorReply = ("nie je možné aktualizovať požiadavku na zvolený deň");
            }
            else
            {
                ViewBag.ErrorReply = ("nepodarilo sa aktualizovať požiadavku");
            }

            return View();
        }

        [HttpPost]
        public IActionResult PhotoManagement(int category, IFormFile file)
        {
            try
            {
                string directory = (enviroment.ContentRootPath + "wwwroot\\image\\gallery\\" + Translator.Categories.ElementAt(category).Key + "\\");
                int? id = PhotoController.GetId();

                if (id == null)
                {
                    throw new Exception();
                }
                else
                {
                    string name = (id + ".jpg");
                    string path = Path.Combine(directory, name);
                    bool copy = PhotoController.CopyImage(path, file);

                    if (copy)
                    {
                        Image image = Image.FromFile(path);

                        if (Validator.IsPictureValid(image.Width, image.Height))
                        {
                            PhotoModel photo = new PhotoModel();
                            photo.TITLE = name;
                            photo.CATEGORY = Translator.Categories.ElementAt(category).Key;
                            photo.ORIENTATION = (image.Width > image.Height ? false : true);

                            if (PhotoController.InsertPhoto(photo))
                            {
                                ViewBag.SuccessReply = ("fotografia bola úspešne nahratá");
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        else
                        {
                            PhotoController.DeleteImage(path);
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch
            {
                ViewBag.ErrorReply = ("nepodarilo sa vložiť fotografiu");
            }

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

            bool? output = UserController.InsertUser(user);

            if (output == true)
            {
                LoginAction(mail, Translator.Access[role]);
                return RedirectToAction("Index", "Home");
            }
            else if (output == false)
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

        public IActionResult Statistics()
        {
            if (Translator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(Variables.Role)).Key >= 2)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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

        public IActionResult PhotoManagement()
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
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.SuccessReply = TempData["SuccessReply"];
                ViewBag.ErrorReply = TempData["ErrorReply"];
                return View();
            }
        }

        public IActionResult AccountDetails() 
        {
            string user = context.HttpContext.Session.GetString(Variables.Mail);
            var request = TempData["User"];
            
            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (request == null)
                {
                    ViewBag.User = user;
                }
                else
                {
                    ViewBag.User = request;
                }

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

        public IActionResult DeletePhoto(int id, string category, string file)
        {
            if (Translator.Access.FirstOrDefault(x => x.Value == context.HttpContext.Session.GetString(Variables.Role)).Key >= 2)
            {
                try
                {
                    string path = Path.Combine((enviroment.ContentRootPath + "wwwroot\\image\\gallery\\" + category + "\\"), file);

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (!PhotoController.DeleteImage(path))
                        {
                            throw new Exception();
                        }
                        
                        if (PhotoController.DeletePhoto(id))
                        {
                            TempData["SuccessReply"] = ("fotografia bola úspešne odstránená");
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                catch
                {
                    TempData["ErrorReply"] = ("fotografiu sa nepodarilo odstrániť");
                }

                return RedirectToAction("PhotoManagement", "Home");
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
                bool? output = RequestController.DeleteRequest(id, admin);

                if (output == true)
                {
                    TempData["SuccessReply"] = ("požiadavka číslo " + id + " bola odstránená");
                }
                else if (output == false)
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
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString(Variables.Mail)))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Id = TempData["Id"];
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
            StatisticsController.InsertStatistic("wedding");
            return View("~/Views/Gallery/Wedding.cshtml");
        }

        public IActionResult Event()
        {
            StatisticsController.InsertStatistic("event");
            return View("~/Views/Gallery/Event.cshtml");
        }

        public IActionResult Car()
        {
            StatisticsController.InsertStatistic("car");
            return View("~/Views/Gallery/Car.cshtml");
        }

        public IActionResult Nature()
        {
            StatisticsController.InsertStatistic("nature");
            return View("~/Views/Gallery/Nature.cshtml");
        }

        public IActionResult Other()
        {
            StatisticsController.InsertStatistic("other");
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