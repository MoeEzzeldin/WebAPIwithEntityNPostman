using AutoMapper;
using System.Data;
using SqlApiPostman.Models.Entities;
using SqlApiPostman.Models.DTOs;

namespace SqlApiPostman.Data.Mappings
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            // Create mappings between Category entity and CategoryDTO
            CreateMap<Category, CategoryDTO>();

            CreateMap<CategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id to allow auto-generation by the database

        }

    }
}
