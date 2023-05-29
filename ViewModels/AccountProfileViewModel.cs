using PustokProject.Models;

namespace PustokProject.ViewModels
{
    public class AccountProfileViewModel
    {
        public ProfileEditViewModel Profile { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
