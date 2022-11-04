using Microsoft.AspNetCore.Http.Features;
using Microsoft.Data.SqlClient;
using SemestralnaPraca.Models;

namespace SemestralnaPraca.Controllers
{
    public static class PhotoResolver
    {
        public static List<PhotoModel> GetGallery(string category)
        {
            List<PhotoModel> horizontalPhotos = new List<PhotoModel>();
            List<PhotoModel> verticalPhotos = new List<PhotoModel>();

            string query = "select * from PHOTOS where CATEGORY='" + category + "'";
            string connectionString = "Data Source=localhost;Persist Security Info=True;User ID=sa;Password=yourStrong(!)Password";

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

                            photo.TITLE = (string)reader[1];
                            photo.CATEGORY = (string)reader[2];
                            photo.ORIENTATION = (string)reader[3];

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
    }
}
