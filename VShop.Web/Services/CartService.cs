using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VShop.Web.Models;
using VShop.Web.Services.Contracts;

namespace VShop.Web.Services;

public class CartService : ICartService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions? _options;
    private const string apiEndpoint = "/api/cart";
    private CartViewModel cartVM = new CartViewModel();

    public CartService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<CartViewModel> GetCartByUserIdAsync(string userId, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.GetAsync($"{apiEndpoint}/getcart/{userId}"))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                cartVM = await JsonSerializer
                              .DeserializeAsync<CartViewModel>
                              (apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return cartVM;
    }

    public async Task<CartViewModel> AddItemToCartAsync(CartViewModel cartVM, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        StringContent content = new StringContent(JsonSerializer.Serialize(cartVM),
                                                Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync($"{apiEndpoint}/addcart/", content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                cartVM = await JsonSerializer
                           .DeserializeAsync<CartViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return cartVM;
    }

    public async Task<CartViewModel> UpdateCartAsync(CartViewModel cartVM, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        CartViewModel cartUpdated = new CartViewModel();

        using (var response = await client.PutAsJsonAsync($"{apiEndpoint}/updatecart/", cartVM))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                cartUpdated = await JsonSerializer
                                  .DeserializeAsync<CartViewModel>
                                  (apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return cartUpdated;
    }

    public async Task<bool> RemoveItemFromCartAsync(int cartId, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.DeleteAsync($"{apiEndpoint}/deletecart/" + cartId))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        return false;
    }

    private static void PutTokenInHeaderAuthorization(string token, HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }


    public Task<bool> ClearCartAsync(string userId, string token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ApplyCouponAsync(CartViewModel cartVM, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        StringContent content = new StringContent(JsonSerializer.Serialize(cartVM),
                                         Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync($"{apiEndpoint}/applycoupon/", content))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }

        return false;

    }

    public async Task<bool> RemoveCouponAsync(string userId, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.DeleteAsync($"{apiEndpoint}/deletecoupon/{userId}"))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }

        return false;

    }
    public Task<CartViewModel> CheckoutAsync(CartHeaderViewModel cartHeader, string token)
    {
        throw new NotImplementedException();
    }
}
