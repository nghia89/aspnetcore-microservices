using AutoMapper;
using Infrastructure.Mappings;
using Infrastructure.Shared.DTOs.Product;
using Product.API.Entities;

namespace Product.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CatalogProduct, ProductDto>();
            CreateMap<CreateProductDto, CatalogProduct>();
            CreateMap<UpdateProductDto, CatalogProduct>()
            .IgnoreAllNonExisting();
        }
    }
}
