using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;
using System.Data;
using System.Security.Policy;

namespace SemestralnaPraca.Controllers
{
    public static class RequestController
    {
        public static List<RequestModel> GetRequests()
        {
            try
            {
                List<RequestModel> requests = new List<RequestModel>();

                string query = ("select ID, USER, CATEGORY, STATUS, DESCRIPTION, " +
                    "FORMAT (CREATED, 'dd-MM-yyyy'), FORMAT (SCHEDULED, 'dd-MM-yyyy') from REQUESTS order by SCHEDULED desc");

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RequestModel request= new RequestModel();

                                request.ID= int.Parse(reader[0].ToString());
                                request.USER = reader[1].ToString();
                                request.CATEGORY = Translator.Categories[reader[2].ToString()];
                                request.STATUS = reader[3].ToString();
                                request.DESCRIPTION = reader[4].ToString();
                                request.CREATED = reader[5].ToString();
                                request.SCHEDULED = reader[6].ToString();

                                requests.Add(request);
                            }
                        }
                    }
                }

                return requests;
            }
            catch
            {
                return null;
            }
        }

        public static RequestModel GetRequest(int id)
        {
            try
            {
                string query = ("select * from REQUESTS where ID = '" + id + "'");

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();

                                RequestModel request = new RequestModel();

                                request.ID = int.Parse(reader[0].ToString());
                                request.USER = reader[1].ToString();
                                request.CATEGORY = Translator.Categories[reader[2].ToString()];
                                request.STATUS = reader[3].ToString();
                                request.DESCRIPTION = reader[4].ToString();
                                request.CREATED = reader[5].ToString();
                                request.SCHEDULED = reader[6].ToString();

                                return request;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        
        public static bool InsertRequest(RequestModel request)
        {
            try
            {
                bool available = true;
                
                string availableQuery = ("select count(*) from REQUESTS where SCHEDULED='" + request.SCHEDULED + "'");
                string createQuery = ("insert into REQUESTS values " +
                    "('" + request.USER + "', '" + request.CATEGORY + "', '1', " + (string.IsNullOrEmpty(request.DESCRIPTION) ? "null" : ("'" + request.DESCRIPTION+ "'")) + ", getdate(), "
                    + (string.IsNullOrEmpty(request.SCHEDULED) ? "null)" : ("'" + request.SCHEDULED + "')")));

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

        public static bool DeleteRequest(int id)
        {
            try
            {
                string query = ("delete from REQUESTS where ID = '" + id + "'");
                
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