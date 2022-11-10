using Microsoft.Data.SqlClient;

namespace SemestralnaPraca.Controllers
{
    public static class RequestController
    {
        public static bool InsertRequest(string user, int category, string description, string date)
        {
            try
            {
                string query = ("insert into REQUESTS values " +
                    "('" + user + "', '" + Translator.Categories.ElementAt(category).Key + "', '1', '" + description + "')");

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
