namespace PustokProject.ViewModels
{
    public class BasketViewModel
    {
        public List<BasketItemViewModel> Items { get; set; }    = new List<BasketItemViewModel>();
        public decimal TotalPrice { get; set; }
    }
}
