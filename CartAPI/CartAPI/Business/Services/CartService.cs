using CartAPI.Business.Interfaces;
using CartAPI.Model;

namespace CartAPI.Business.Services
{
    public class CartService : ICartService
    {
        private readonly IGenericCacheService _cacheService;
        private readonly TimeSpan _cartExpiration = TimeSpan.FromDays(2);

        public CartService(IGenericCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<Cart> GetCartAsync(string userId)
        {
            var key = CacheKeys.Cart.GetKey(userId);
            var cart = await _cacheService.GetAsync<Cart>(key);
            return cart ?? new Cart { UserId = userId };
        }

        public async Task AddItemAsync(string userId, CartItem item)
        {
            var key = CacheKeys.Cart.GetKey(userId);
            var cart = await GetCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            await _cacheService.SetAsync(key, cart, _cartExpiration);
        }

        public async Task RemoveItemAsync(string userId, string productId)
        {
            var key = CacheKeys.Cart.GetKey(userId);
            var cart = await GetCartAsync(userId);
            cart.Items.RemoveAll(i => i.ProductId == productId);
            await _cacheService.SetAsync(key, cart, _cartExpiration);
        }

        public async Task ClearCartAsync(string userId)
        {
            var key = CacheKeys.Cart.GetKey(userId);
            await _cacheService.RemoveAsync(key);
        }

        public async Task UpdateItemQuantityAsync(string userId, string productId, int quantity)
        {
            var key = CacheKeys.Cart.GetKey(userId);
            var cart = await GetCartAsync(userId);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                await _cacheService.SetAsync(key, cart, _cartExpiration);
            }
        }
    }
}
