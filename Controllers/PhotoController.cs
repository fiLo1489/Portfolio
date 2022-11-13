using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;

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

                                photo.ID = int.Parse(reader[0].ToString());
                                photo.TITLE = reader[1].ToString();
                                photo.CATEGORY = reader[2].ToString();
                                photo.ORIENTATION = Translator.Orientation[reader[3].ToString()];

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

                                photo.ID = int.Parse(reader[0].ToString());
                                photo.TITLE = reader[1].ToString();
                                photo.CATEGORY = reader[2].ToString();
                                photo.ORIENTATION = Translator.Orientation[reader[3].ToString()];

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

        public static List<string> GetCategories()
        {
            return Translator.Categories.Values.ToList();
        }
    }
}