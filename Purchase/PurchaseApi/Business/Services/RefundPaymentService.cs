using PurchaseApi.Business.Interfaces;
using PurchaseApi.Infrastructure.Data.Interfaces;
using PurchaseApi.Models.Entities;
using Stripe;

namespace PurchaseApi.Business.Services
{
    public class RefundPaymentService : IRefundPurchaseService
    {
        private readonly IRepository<Refundment> _repository;  // Veritabanı kaydı için repository
        public RefundPaymentService(IRepository<Refundment> repository)
        {
            _repository = repository;
        }
        public async Task RefundPaymentAsync(string stripeId, decimal amount)
        {
            try
            {
                StripeConfiguration.ApiKey = "...";
                var options = new RefundCreateOptions
                {
                    Amount = (long)amount,
                    Charge = stripeId
                };
                var service = new RefundService();
                var refund = await service.CreateAsync(options);
                Console.WriteLine($"Refund success: {refund.Id}");
                // Veri tabanı işlemleri.
                //*********
            }
            catch (Exception ex)//Controllera taşınacak..
            {
                Console.WriteLine($"Refund failed: {stripeId}");
                throw;
            }
        }

    }
}
