namespace PustokProject.ViewModels
{
    public class OrderViewModel
    {
        public List<CheckoutItem> CheckoutItems { get; set; } = new List<CheckoutItem>();
        public decimal TotalPrice { get; set; }
        public OrderCreateViewModel Order { get; set; }
    }
}
