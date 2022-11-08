using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;
using System.Text;

namespace SemestralnaPraca.Controllers
{
    public static class DataResolver
    {
        public static List<PhotoModel> GetGallery(string category)
        {
            List<PhotoModel> horizontalPhotos = new List<PhotoModel>();
            List<PhotoModel> verticalPhotos = new List<PhotoModel>();

            string query = "select * from PHOTOS where CATEGORY='" + category + "'";
            string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Local"];

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

        public static List<string> GetCategories()
        {
            return DatabaseTranslator.Categories.Values.ToList();
        }

        public static string Hash(string input)
        {
            if (!string.IsNullOrEmpty(input))
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
            else
            {
                return string.Empty;
            }
        }
    }
}
