using Microsoft.AspNetCore.Mvc;
using PurchaseApi.Models.DTO;

namespace PurchaseApi.Business.Interfaces
{
    public interface IPurchaseService
    {
        Task<IActionResult> CreatePurchaseAsync(PurchaseRequest request);
    }
}
