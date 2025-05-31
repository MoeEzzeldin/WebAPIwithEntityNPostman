using AutoMapper;
using SqlApiPostman.Models.Entities;
using SqlApiPostman.Models.DTOs;

namespace SqlApiPostman.Data.Mappings
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            // Create mappings between Product entity and ProductDTO
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();
        }
    }
}
