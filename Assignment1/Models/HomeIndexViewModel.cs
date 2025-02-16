namespace Assignment1.Models
{
    public class HomeIndexViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<OrderList> OrderLists { get; set; } // Include OrderLists if needed
        public List<Cart> Carts { get; set; }
    }
}