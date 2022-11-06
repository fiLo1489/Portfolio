using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NuGet.DependencyResolver;
using System.Text;

namespace SemestralnaPraca.Controllers
{
    public static class DataHandler
    {
        public static string CheckPassword(string password)
        { 
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                result += "nebolo zadané heslo, ";
            }
            else
            {
                if (password.Contains("select ") || password.Contains("delete ") || password.Contains("alter ") || password.Contains("update "))
                {
                    result += "zadané heslo obashuje nepovolené kľúčové slová, ";
                }

                if (!(password.Contains('!') || password.Contains('.') || password.Contains('&') || password.Contains('#') || password.Contains('/')) ||
                    !(password.Any(char.IsDigit) &&
                    password.Any(char.IsUpper) &&
                    password.Any(char.IsLower)))
                {
                    result += "heslo musí obsahovať veľký a malý znak, číslo a bezpečnostný znak (! . & # /), ";
                }
            }

            return result;
        }

        public static string CheckMail(string mail)
        {
            string result = string.Empty;
            
            if (string.IsNullOrWhiteSpace(mail))
            {
                result += "nebol zadaný mail, ";
            }
            else
            {
                if (!mail.Trim().Contains('@') || !mail.Trim().Contains('.'))
                {
                    result += "zadaný mail má nesprávny tvar, ";
                }

                if (mail.Contains("select ") || mail.Contains("delete ") || mail.Contains("alter ") || mail.Contains("update "))
                {
                    result += "zadaný mail obashuje nepovolené kľúčové slová, ";
                }

                if (mail.Length > 255)
                {
                    result += "mail je príliš dlhý";
                }
            }

            return result;
        }

        public static string CheckPhone(string phone)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(phone))
            {
                result += "nebolo zadaný telefónne číslo, ";
            }
            else
            {
                if (!phone.StartsWith('+') || !phone.Substring(1).All(char.IsDigit))
                {
                    result += "zadané telefónne číslo má nesprávny tvar, ";
                }

                if (phone.Length > 14)
                {
                    result += "číslo je príliš dlhé";
                }
            }

            return result;
        }

        public static string Hash(string input)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var output = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (byte theByte in crypto)
            {
                output.Append(theByte.ToString("x2"));
            }
            return output.Append("pht").ToString();
        }
    }
}
