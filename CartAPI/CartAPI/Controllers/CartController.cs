using CartAPI.Business.Interfaces;
using CartAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<Cart>> GetCart(string userId)
        {
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("{userId}/items")]
        public async Task<IActionResult> AddItem(string userId, CartItem item)
        {
            await _cartService.AddItemAsync(userId, item);
            return Ok();
        }

        [HttpPut("{userId}/items/{productId}")]
        public async Task<IActionResult> UpdateItemQuantity(string userId, string productId, [FromBody] int quantity)
        {
            await _cartService.UpdateItemQuantityAsync(userId, productId, quantity);
            return Ok();
        }

        [HttpDelete("{userId}/items/{productId}")]
        public async Task<IActionResult> RemoveItem(string userId, string productId)
        {
            await _cartService.RemoveItemAsync(userId, productId);
            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            await _cartService.ClearCartAsync(userId);
            return Ok();
        }
    }
}
