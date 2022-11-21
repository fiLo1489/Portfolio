using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;
using System.ComponentModel;

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

        public static Dictionary<string, int> GetStatisticsDate(string date)
        {
            try
            {
                Dictionary<string, int> statisticsDate = new Dictionary<string, int>();

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
                                statisticsDate.Add(Translator.Categories[reader[0].ToString()], Convert.ToInt32(reader[1].ToString()));
                            }
                        }
                    }
                }

                return statisticsDate;
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<int, int> GetStatisticsMonth(int year, int month)
        {
            try
            {
                Dictionary<int, int> statisticsMonth = new Dictionary<int, int>();

                string query = "select DAY(DATE), count(*), DATE from VISITS where MONTH(DATE) = '" + month + "'  and YEAR(DATE) = '" + year + "' group by DATE";

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                statisticsMonth.Add(Convert.ToInt32(reader[0].ToString()), Convert.ToInt32(reader[1].ToString()));
                            }
                        }
                    }
                }

                return statisticsMonth;
            }
            catch
            {
                return null;
            }
        }
    }
}