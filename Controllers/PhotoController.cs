using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;
using SemestralnaPraca.Views.Components;
using System.Drawing;
using System.Transactions;

namespace SemestralnaPraca.Controllers
{
    public static class PhotoController
    {
        public static List<PhotoModel> GetGallery(string category)
        {
            try
            {
                List<PhotoModel> horizontalPhotos = new List<PhotoModel>();
                List<PhotoModel> verticalPhotos = new List<PhotoModel>();

                string query = ("select * from PHOTOS where CATEGORY='" + category + "'");

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PhotoModel photo = new PhotoModel();

                                photo.ID = Convert.ToInt32(reader[0].ToString());
                                photo.TITLE = reader[1].ToString();
                                photo.CATEGORY = reader[2].ToString();
                                photo.ORIENTATION = Convert.ToBoolean(Convert.ToInt16(reader[3].ToString()));

                                if (photo.ORIENTATION)
                                {
                                    verticalPhotos.Add(photo);
                                }
                                else
                                {
                                    horizontalPhotos.Add(photo);
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

        public static List<PhotoModel> GetPhotos()
        {
            try
            {
                List<PhotoModel> photos = new List<PhotoModel>();

                string query = ("select * from PHOTOS order by CATEGORY");

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PhotoModel photo = new PhotoModel();

                                photo.ID = Convert.ToInt32(reader[0].ToString());
                                photo.TITLE = reader[1].ToString();
                                photo.CATEGORY = reader[2].ToString();
                                photo.ORIENTATION = Convert.ToBoolean(Convert.ToInt16(reader[3].ToString()));

                                photos.Add(photo);
                            }
                        }
                    }
                }

                return photos;
            }
            catch
            {
                return null;
            }
        }

        public static bool InsertPhoto(PhotoModel photo)
        {
            try
            {
                string query = "insert into PHOTOS values ('" + photo.TITLE + "', '" + photo.CATEGORY + "', '" + (photo.ORIENTATION ? "1" : "0") + "')";

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

        public static bool DeletePhoto(int id)
        {
            try
            {
                string query = ("delete from PHOTOS where ID = '" + id + "'");

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

        public static int? GetId()
        {
            try
            {
                int value = 0;

                string query = "select (max(ID) + 1) from PHOTOS";

                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"]))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        value = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                return value;
            }
            catch
            {
                return null;
            }
        }

        public static bool CopyImage(string path, IFormFile file)
        {
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.CopyTo(fileStream);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DeleteImage(string path)
        {
            try
            {
                if ((File.Exists(path)))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(path);
                    return true;
                }
                else
                { 
                    return false;
                }
            }
            catch 
            {
                return false;
            }
        }

        public static List<string> GetCategories()
        {
            return Translator.Categories.Values.ToList();
        }
    }
}