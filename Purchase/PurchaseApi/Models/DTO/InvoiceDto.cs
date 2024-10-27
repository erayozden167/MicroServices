namespace PurchaseApi.Models.DTO
{
    public class InvoiceDto
    {
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string PaymentId { get; set; }
    }
}
