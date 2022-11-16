using System.Text.RegularExpressions;

namespace SemestralnaPraca.Controllers
{
    public static class Validator
    {
        public static bool IsMailValid(string value)
        {
            if (Regex.IsMatch(value, @"([a-z]||[0-9])(@{1})([a-z]+)(\.)([a-z])"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsPhoneValid(string value)
        {
            if (Regex.IsMatch(value, @"(^\+)([0-9]{10,13})"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsPasswordValid(string value)
        {
            if (Regex.IsMatch(value, @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#.!&/]).+"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsSqlInjection(string value)
        {
            if (Regex.IsMatch(value, @"(alter|update|select|delete)"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}