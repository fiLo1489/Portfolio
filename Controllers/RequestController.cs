using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;

namespace SemestralnaPraca.Controllers
{
    public static class RequestController
    {
        public static List<RequestModel> GetRequests()
        {
            try
            {
                List<RequestModel> requests = new List<RequestModel>();

                string query = ("select ID, [USER], CATEGORY, STATUS, DESCRIPTION, " +
                    "FORMAT (CREATED, 'dd/MM/yyyy'), FORMAT (SCHEDULED, 'dd/MM/yyyy'), RESULT from REQUESTS order by SCHEDULED asc");

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
                                request.STATUS = Translator.Status[int.Parse(reader[3].ToString())];
                                request.DESCRIPTION = reader[4].ToString();
                                request.CREATED = reader[5].ToString();
                                request.SCHEDULED = reader[6].ToString();
                                request.RESULT = reader[7].ToString();

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

        public static RequestModel GetRequest(string id)
        {
            try
            {
                string query = ("select ID, [USER], CATEGORY, STATUS, DESCRIPTION, " +
                    "FORMAT (CREATED, 'dd/MM/yyyy'), FORMAT (SCHEDULED, 'dd/MM/yyyy'), RESULT from REQUESTS where ID = '" + id + "'");

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
                                request.STATUS = Translator.Status[int.Parse(reader[3].ToString())];
                                request.DESCRIPTION = reader[4].ToString();
                                request.CREATED = reader[5].ToString();
                                request.SCHEDULED = reader[6].ToString();
                                request.RESULT = reader[7].ToString();

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
        
        public static bool? InsertRequest(RequestModel request)
        {
            if (!Validator.IsSqlInjection(request.DESCRIPTION))
            {
                try
                {
                    bool available = true;

                    string availableQuery = ("select count(*) from REQUESTS where SCHEDULED='" + request.SCHEDULED + "'");
                    string createQuery = ("insert into REQUESTS values " +
                        "('" + request.USER + "', '" + request.CATEGORY + "', '1', " + (string.IsNullOrEmpty(request.DESCRIPTION) ? "null" : ("'" + request.DESCRIPTION + "'")) +
                        ", getdate(), '" + request.SCHEDULED + "', " + (string.IsNullOrEmpty(request.RESULT) ? "null)" : ("'" + request.RESULT + "')")));

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
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static bool? DeleteRequest(string id, bool admin)
        {
            try
            {
                bool state = false;
                
                string deleteQuery = ("delete from REQUESTS where ID = '" + id + "'");
                string stateQuery = ("select count(*) from REQUESTS where ID = '" + id + "' and STATUS = '1'");
                
                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(stateQuery, connection))
                    {
                        state = (Convert.ToInt32(cmd.ExecuteScalar()) != 0 ? true : false);
                    }

                    if (admin || state)
                    {
                        using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool? UpdateRequest(RequestModel request, bool admin)
        {
            if (!Validator.IsSqlInjection(request.DESCRIPTION))
            {
                try
                {
                    bool available = true;

                    string availableQuery = ("select count(*) from REQUESTS where SCHEDULED = '" + request.SCHEDULED + "' and ID != '" + request.ID + "'");
                    string updateQuery;

                    if (admin)
                    {
                        updateQuery = ("update REQUESTS set SCHEDULED = '" + request.SCHEDULED + "', DESCRIPTION = " +
                            (string.IsNullOrEmpty(request.DESCRIPTION) ? "NULL" : ("'" + request.DESCRIPTION + "'")) + ", STATUS = '" +
                            Translator.Status.FirstOrDefault(x => x.Value == request.STATUS).Key + "', RESULT = " +
                            (string.IsNullOrEmpty(request.RESULT) ? "NULL" : ("'" + request.RESULT + "'")) + " where ID = '" + request.ID + "'");
                    }
                    else if (!admin && request.STATUS == Translator.Status[1])
                    {
                        updateQuery = ("update REQUESTS set SCHEDULED = '" + request.SCHEDULED + "', DESCRIPTION = " +
                            (string.IsNullOrEmpty(request.DESCRIPTION) ? "NULL" : ("'" + request.DESCRIPTION + "'")) + " where ID = '" + request.ID + "'");
                    }
                    else
                    {
                        return null;
                    }

                    using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                    {
                        connection.Open();

                        using (SqlCommand cmd = new SqlCommand(availableQuery, connection))
                        {
                            available = (Convert.ToInt32(cmd.ExecuteScalar()) == 0 ? true : false);
                        }

                        if (available)
                        {
                            using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    return available;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}