using System.Text.RegularExpressions;
using System.Transactions;

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

        public static bool IsPictureValid(int width, int heigth)
        {
            if (((width / heigth) == (16 / 10))|| ((heigth / width) == (16 / 10)))
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