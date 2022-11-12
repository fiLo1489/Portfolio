using Microsoft.Data.SqlClient;

namespace SemestralnaPraca.Controllers
{
    public static class RequestController
    {
        public static bool InsertRequest(string user, int category, string description, string date)
        {
            try
            {
                bool available = true;
                
                string availableQuery = ("select count(*) from REQUESTS where SCHEDULED='" + date + "'");
                string createQuery = ("insert into REQUESTS values " +
                    "('" + user + "', '" + Translator.Categories.ElementAt(category).Key + "', '1', " + (string.IsNullOrEmpty(date) ? "null" : ("'" + description + "'")) + ", getdate(), "
                    + (string.IsNullOrEmpty(date) ? "null)" : ("'" + date + "')")));

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(availableQuery, connection))
                    {
                        available = (Convert.ToInt32(cmd.ExecuteScalar()) == 0 ? true : false);
                    }

                    if (available)
                    { 
                        using (SqlCommand cmd = new SqlCommand(createQuery, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return available;
            }
            catch
            {
                return false;
            }
        }
    }
}