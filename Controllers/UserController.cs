using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;
using System.Text;

namespace SemestralnaPraca.Controllers
{
    public static class UserController
    {
        public static List<UserModel> GetUsers(int role)
        {
            try
            {
                List<UserModel> users = new List<UserModel>();

                string query = ("select * from CREDENTIALS where ROLE <= '" + role + "'");

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserModel user = new UserModel();

                                user.MAIL = reader[0].ToString();
                                user.PASSWORD = reader[1].ToString();
                                user.NAME = reader[2].ToString();
                                user.SURNAME = reader[3].ToString();
                                user.PHONE = reader[4].ToString();
                                user.ROLE = int.Parse(reader[5].ToString());

                                users.Add(user);
                            }
                        }
                    }
                }

                return users;
            }
            catch
            {
                return null;
            }
        }

        public static UserModel GetUser(string mail)
        {
            try
            {
                string query = ("select * from CREDENTIALS where MAIL = '" + mail + "'");

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

                                UserModel user = new UserModel();

                                user.MAIL = reader[0].ToString();
                                user.PASSWORD = reader[1].ToString();
                                user.NAME = reader[2].ToString();
                                user.SURNAME = reader[3].ToString();
                                user.PHONE = reader[4].ToString();
                                user.ROLE = int.Parse(reader[5].ToString());

                                return user;
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

        public static bool UpdateUser(UserModel user)
        {
            if (!Validator.IsSqlInjection(user.NAME) && !Validator.IsSqlInjection(user.SURNAME) && Validator.IsPhoneValid(user.PHONE))
            {
                try
                {
                    string query = ("update CREDENTIALS set NAME = '" + user.NAME + "', SURNAME = '" + user.SURNAME + "', PHONE = '"
                    + user.PHONE + "', ROLE = '" + user.ROLE + "'");

                    if (!string.IsNullOrEmpty(user.PASSWORD))
                    {
                        if (Validator.IsPasswordValid(user.PASSWORD))
                        {
                            query += (", PASSWORD = '" + user.PASSWORD + "'");
                        }
                        else
                        {
                            return false;
                        }
                    }

                    query += (" where MAIL = '" + user.MAIL + "'");

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
            else
            {
                return false;
            }
        }

        public static bool DeleteUser(string mail)
        {
            try
            {
                string query = ("delete from CREDENTIALS where MAIL='" + mail + "'");

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

        public static bool? InsertUser(UserModel user)
        {
            if (!Validator.IsSqlInjection(user.NAME) && !Validator.IsSqlInjection(user.SURNAME) && Validator.IsPhoneValid(user.PHONE) && Validator.IsPasswordValid(user.PASSWORD))
            {
                try
                {
                    bool exists = false;

                    string existsQuery = "select count(*) from CREDENTIALS where MAIL='" + user.MAIL + "'";
                    string registerQuery = "insert into CREDENTIALS values " +
                        "('" + user.MAIL + "', '" + GetPassword(user.PASSWORD) + "', '" + user.NAME + "', '" + user.SURNAME + "', '" + user.PHONE + "', '" + user.ROLE + "')";

                    using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                    {
                        connection.Open();

                        using (SqlCommand cmd = new SqlCommand(existsQuery, connection))
                        {
                            exists = (Convert.ToInt32(cmd.ExecuteScalar()) != 0 ? true : false);
                        }

                        if (!exists)
                        {
                            using (SqlCommand cmd = new SqlCommand(registerQuery, connection))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    return exists;
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

        public static string GetPassword(string input)
        {
            try
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
            catch
            {
                return string.Empty;
            }
        }
    }
}
