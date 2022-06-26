using System.Net.Http.Headers;
using System.Text.Json;
using VShop.Web.Models;
using VShop.Web.Services.Contracts;

namespace VShop.Web.Services;

public class CategoryService : ICategoryService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions _options;
    private const string apiEndpoint = "/api/categories/";

    public CategoryService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<IEnumerable<CategoryViewModel>> GetAllCategories(string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        IEnumerable<CategoryViewModel> categories;

        using (var response = await client.GetAsync(apiEndpoint))
        {

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                categories = await JsonSerializer
                          .DeserializeAsync<IEnumerable<CategoryViewModel>>(apiResponse, _options);
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
        return categories;
    }
}
