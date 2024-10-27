namespace PurchaseApi.Models.DTO
{
    public class PurchaseRequest
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public string StripeToken { get; set; }
        public decimal Amount { get; set; }
        public bool IsAvailable { get; set; }
    }
}
