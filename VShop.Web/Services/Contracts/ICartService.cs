using VShop.Web.Models;

namespace VShop.Web.Services.Contracts;

public interface ICartService
{
    Task<CartViewModel> GetCartByUserIdAsync(string userId, string token);
    Task<CartViewModel> AddItemToCartAsync(CartViewModel cartVM, string token);
    Task<CartViewModel> UpdateCartAsync(CartViewModel cartVM, string token);
    Task<bool> RemoveItemFromCartAsync(int cartId, string token);

    //implementação futura
    Task<bool> ApplyCouponAsync(CartViewModel cartVM, string couponCode, string token);
    Task<bool> RemoveCouponAsync(string userId, string token);
    Task<bool> ClearCartAsync(string userId, string token);

    Task<CartViewModel> CheckoutAsync(CartHeaderViewModel cartHeader, string token);
}
