using Microsoft.AspNetCore.Identity;

namespace SemestralnaPraca.Models
{
    public class UserModel : IdentityUser
    {
        public string LOGIN { get; set; }
        public string PASSWORD { get; set; }
        public string ROLE { get; set; }
        public string MAIL { get; set; }
        public string TELEPHONE { get; set; }
        public string NAME { get; set; }
        public string SURNAME { get; set; }
    }
}
