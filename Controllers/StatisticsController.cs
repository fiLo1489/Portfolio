using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;

namespace SemestralnaPraca.Controllers
{
    public static class StatisticsController
    {
        public static void InsertStatistic(string category)
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

        public static Dictionary<string, int> GetStatistics(string date)
        {
            try
            {
                Dictionary<string, int> statistics = new Dictionary<string, int>();

                string query = "select TITLE, count(*) from VISITS ";
                if (!string.IsNullOrEmpty(date))
                {
                    query += ("where DATE = '" + date + "' ");
                }
                query += "group by TITLE order by TITLE";

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                statistics.Add(Translator.Categories[reader[0].ToString()], Convert.ToInt32(reader[1].ToString()));
                            }
                        }
                    }
                }

                return statistics;
            }
            catch
            {
                return null;
            }
        }
    }
}