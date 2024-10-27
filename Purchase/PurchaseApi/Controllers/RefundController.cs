using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PurchaseApi.Business.Services;

namespace PurchaseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly RefundPaymentService _refundPaymentService;
        public RefundController(RefundPaymentService refundPaymentService)
        {
            _refundPaymentService = refundPaymentService;
        }
        public async Task<IActionResult> RefundPaymentAsync()
        {
            return await _refundPaymentService.
        }
    }
}
