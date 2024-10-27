using Microsoft.AspNetCore.Mvc;
using PurchaseApi.Models.DTO;

namespace PurchaseApi.Business.Interfaces
{
    public interface IRefundPurchaseService
    {
        Task RefundPaymentAsync(string stripeId, decimal amount);
    }
}
