using System.ComponentModel.DataAnnotations;

namespace PurchaseApi.Models.Entities
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string StripePaymentId { get; set; } // stripe ödeme bilgileri.
    }
}
