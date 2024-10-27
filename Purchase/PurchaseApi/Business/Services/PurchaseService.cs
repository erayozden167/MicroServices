using Microsoft.AspNetCore.Mvc;
using PurchaseApi.Business.Interfaces;
using PurchaseApi.Infrastructure.Data.Interfaces;
using PurchaseApi.Models.DTO;
using PurchaseApi.Models.Entities;
using Stripe;

namespace PurchaseApi.Business.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly RabbitMQPublisher _publisher;
        private readonly IRepository<Purchase> _repository;  // Veritabanı kaydı için repository

        public PurchaseService(RabbitMQPublisher publisher, IRepository<Purchase> repository)
        {
            _publisher = publisher;
            _repository = repository;
        }

        public async Task<IActionResult> CreatePurchaseAsync(PurchaseRequest request)
        {
            // Stripe ile ödeme işlemi
            var paymentResult = await ProcessPaymentAsync(request.StripeToken, request.Amount);
            if (!paymentResult.success)
            {
                return new BadRequestObjectResult("Payment failed");
            }

            // RabbitMQ ile stok güncelleme mesajı yayınla
            _publisher.PublishStockUpdateMessage(request.ProductId, request.Quantity);

            // Satın alma işlemi veritabanına ekleniyor
            var purchase = new Purchase
            {
                ProductId = request.ProductId,
                UserId = request.UserId,  // Kullanıcı bilgisi
                Amount = request.Amount,
                Quantity = request.Quantity,
                PurchaseDate = DateTime.Now,
                StripePaymentId = paymentResult.paymentId  // Stripe ödeme ID'si
            };

            await _repository.AddAsync(purchase);

            // Fatura bilgilerini derliyoruz şuanlık basit yapıda..
            var invoice = new InvoiceDto
            {
                PurchaseId = purchase.Id,
                ProductId = purchase.ProductId,
                UserId = purchase.UserId,
                Amount = purchase.Amount,
                Quantity = purchase.Quantity,
                PurchaseDate = purchase.PurchaseDate,
                PaymentId = purchase.StripePaymentId
            };

            return new OkObjectResult(new { message = "Purchase completed successfully", invoice });
        }

        private async Task<(bool success, string paymentId)> ProcessPaymentAsync(string stripeToken, decimal amount)
        {
            try
            {
                StripeConfiguration.ApiKey = "Stripe-Secret";
                var options = new ChargeCreateOptions
                {
                    Amount = (long)(amount * 100),  // Dolar olarak stripe işlemi
                    Currency = "usd",
                    Source = stripeToken,
                    Description = "E-commerce product purchase"
                };
                var service = new ChargeService();
                var charge = await service.CreateAsync(options);

                return (true, charge.Id);  // Başarılı olursa ödeme ID'si döndürülüyor
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment error: {ex.Message}");
                return (false, null);
            }
        }

       
    }
}
