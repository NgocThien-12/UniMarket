using UniMarket.Models;
using UniMarket.ViewModels;

namespace UniMarket.Services
{
    public interface ICartService
    {
        Task<CartViewModel> GetCartAsync(string? userId);
        Task<(bool Success, string Message)> AddToCartAsync(string? userId, int productId, int quantity = 1);
        Task<(bool Success, string Message)> UpdateQuantityAsync(string? userId, int productId, int quantity);
        Task<bool> RemoveFromCartAsync(string? userId, int productId);
        Task ClearCartAsync(string? userId);
        Task<int> GetCartItemCountAsync(string? userId);
        Task<(bool Success, string Message, Order? Order)> CheckoutAsync(string userId, CheckoutViewModel model);
    }
}

