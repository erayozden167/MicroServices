using Microsoft.AspNetCore.Mvc;
using PurchaseApi.Business.Interfaces;
using PurchaseApi.Models.DTO;

[ApiController]
[Route("api/[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePurchase([FromBody] PurchaseRequest request)
    {
        return await _purchaseService.CreatePurchaseAsync(request);
    }
}
