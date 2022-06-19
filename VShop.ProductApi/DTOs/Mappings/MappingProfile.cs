using AutoMapper;
using VShop.ProductApi.Models;

namespace VShop.ProductApi.DTOs.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
        
        CreateMap<ProductDTO, Product>();
        CreateMap<Product, ProductDTO>()
         .ForMember(x => x.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        
    }
}
