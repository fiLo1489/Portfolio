using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;

namespace SemestralnaPraca.Controllers
{
    public static class StatisticsController
    {
        public static void Visit(string category)
        {
            string query = ("insert into VISITS values ('" + category + "', getdate())");

            using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}