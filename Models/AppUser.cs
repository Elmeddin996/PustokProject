using Microsoft.AspNetCore.Identity;

namespace PustokProject.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set;}
        public bool IsAdmin { get; set;}
        public string Adress { get; set;}
        public string Phone { get; set;}
    }
}
