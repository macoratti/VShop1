using VShop.Web.Models;

namespace VShop.Web.Services.Contracts;

public interface IProductService
{
    Task<IEnumerable<ProductViewModel>> GetAllProducts(string token);
    Task<ProductViewModel> FindProductById(int id, string token);
    Task<ProductViewModel> CreateProduct(ProductViewModel productVM, string token);
    Task<ProductViewModel> UpdateProduct(ProductViewModel productVM, string token);
    Task<bool> DeleteProductById(int id, string token);
}
