using AutoMapper;
using Inventory.Product.API.Entities;
using Shared.DTOs.InVentory;

namespace Inventory.Product.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InventoryEntry, InventoryEntryDto>().ReverseMap();
            
        }
    }
}
