using CartAPI.Model;

namespace CartAPI.Business.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(string userId);
        Task AddItemAsync(string userId, CartItem item);
        Task RemoveItemAsync(string userId, string productId);
        Task ClearCartAsync(string userId);
        Task UpdateItemQuantityAsync(string userId, string productId, int quantity);
    }
}
