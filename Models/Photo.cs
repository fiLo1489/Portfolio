namespace SemestralnaPraca.Models
{
    public class Photo
    {
        public int ID { get; set; }
        public string TITLE { get; set; }
        public bool ORIENTATION { get; set; }
        public string CATEGORY { get; set; }

        public Photo(int id, string title, bool orientation, string category)
        {
            ID = id;
            TITLE = title;
            ORIENTATION = orientation;
            CATEGORY = category;
        }
    }
}
