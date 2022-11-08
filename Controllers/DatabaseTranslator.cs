namespace SemestralnaPraca.Controllers
{
    public static class DatabaseTranslator
    {
        public static Dictionary<string, string> Orientation = new Dictionary<string, string>()
        {
            { "1", "vertical" },
            { "0", "horizontal" }
        };

        public static Dictionary<int, string> Status = new Dictionary<int, string>()
        {
            { 1, "čakajúce" },
            { 2, "schválené" },
            { 3, "v úprave" },
            { 4, "dokončené" }
        };

        public static Dictionary<int, string> Access = new Dictionary<int, string>()
        {
            { 1, "používateľ" },
            { 2, "administrátor" },
            { 3, "vlastník" }
        };

        public static Dictionary<string, string> Categories = new Dictionary<string, string>()
        {
            { "wedding", "svadby" },
            { "event", "podujatia" },
            { "car", "vozidlá" },
            { "nature", "príroda" },
            { "other", "iné" }
        };
    }
}
