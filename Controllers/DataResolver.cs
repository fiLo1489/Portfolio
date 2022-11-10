using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace SemestralnaPraca.Controllers
{
    public static class DataResolver
    {
        public static string connectionString => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"];

        public static List<PhotoModel> GetGallery(string category)
        {
            try
            {
                List<PhotoModel> horizontalPhotos = new List<PhotoModel>();
                List<PhotoModel> verticalPhotos = new List<PhotoModel>();

                string query = ("select * from PHOTOS where CATEGORY='" + category + "'");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PhotoModel photo = new PhotoModel();

                                photo.TITLE = reader[1].ToString();
                                photo.CATEGORY = reader[2].ToString();
                                photo.ORIENTATION = DatabaseTranslator.Orientation[reader[3].ToString()];

                                if (photo.ORIENTATION.Equals("horizontal"))
                                {
                                    horizontalPhotos.Add(photo);
                                }
                                else
                                {
                                    verticalPhotos.Add(photo);
                                }
                            }
                        }
                    }
                }

                List<PhotoModel> photos = new List<PhotoModel>();

                int verticalSize = ((verticalPhotos.Count % 2 == 0) ? verticalPhotos.Count : (verticalPhotos.Count - 1));
                int horizontalSize = ((horizontalPhotos.Count % 2 == 0) ? horizontalPhotos.Count : (horizontalPhotos.Count - 1));
                int verticalCounter = 0;
                int horizontalCounter = 0;

                bool orientation = false;

                while ((verticalCounter + horizontalCounter) != (verticalSize + horizontalSize))
                {
                    if (orientation)
                    {
                        if (verticalSize != verticalCounter)
                        {
                            photos.Add(verticalPhotos[verticalCounter]);
                            photos.Add(verticalPhotos[verticalCounter + 1]);
                            verticalCounter = (verticalCounter + 2);
                        }
                    }
                    else
                    {
                        if (horizontalSize != horizontalCounter)
                        {
                            photos.Add(horizontalPhotos[horizontalCounter]);
                            photos.Add(horizontalPhotos[horizontalCounter + 1]);
                            horizontalCounter = (horizontalCounter + 2);
                        }
                    }

                    orientation = !orientation;
                }

                if (verticalCounter != verticalPhotos.Count)
                {
                    photos.Add(verticalPhotos[verticalPhotos.Count - 1]);
                }

                if (horizontalCounter != horizontalPhotos.Count)
                {
                    photos.Add(horizontalPhotos[horizontalPhotos.Count - 1]);
                }

                return photos;
            }
            catch
            {
                return null;
            }
        }

        public static List<UserModel> GetUsers(string current, int role)
        {
            try
            {
                List<UserModel> users = new List<UserModel>();

                string query = ("select * from CREDENTIALS where MAIL != '" + current + "' and ROLE <= '" + role + "'");

                using (SqlConnection connection = new SqlConnection(connectionString))
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
                string query = ("select * from CREDENTIALS where MAIL='" + mail + "'");

                using (SqlConnection connection = new SqlConnection(connectionString))
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
            try
            {
                string query = ("update CREDENTIALS set NAME = '" + user.NAME + "', SURNAME = '" + user.SURNAME + "', PHONE = '"
                + user.PHONE + "', ROLE = '" + user.ROLE + "'");

                if (!string.IsNullOrEmpty(user.PASSWORD))
                {
                    query += (", PASSWORD = '" + user.PASSWORD + "'");
                }

                query += (" where MAIL = '" + user.MAIL + "'");

                using (SqlConnection connection = new SqlConnection(connectionString))
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

        public static bool DeleteUser(string mail)
        {
            try
            {
                string query = ("delete from CREDENTIALS where MAIL='" + mail + "'");

                using (SqlConnection connection = new SqlConnection(connectionString))
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

        public static bool InsertRequest(string user, int category, string description)
        {
            try
            {
                string query = ("insert into REQUESTS values " +
                    "('" + user + "', '" + DatabaseTranslator.Categories.ElementAt(category).Key + "', '1', '" + description + "')");

                using (SqlConnection connection = new SqlConnection(connectionString))
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

        public static bool InsertUser(UserModel user)
        {
            try
            {
                bool exists = false;

                string existsQuery = "select count(*) from CREDENTIALS where MAIL='" + user.MAIL + "'";
                string registerQuery = "insert into CREDENTIALS values " +
                    "('" + user.MAIL + "', '" + DataResolver.Hash(user.PASSWORD) + "', '" + user.NAME + "', '" + user.SURNAME + "', '" + user.PHONE + "', '" + user.ROLE + "')";

                using (SqlConnection connection = new SqlConnection(connectionString))
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
                return false;
            }
        }

        public static List<string> GetCategories()
        {
            return DatabaseTranslator.Categories.Values.ToList();
        }

        public static string Hash(string input)
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
