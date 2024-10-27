namespace CartAPI.Model
{
    public class Cart
    {
        public string UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
    }
}
