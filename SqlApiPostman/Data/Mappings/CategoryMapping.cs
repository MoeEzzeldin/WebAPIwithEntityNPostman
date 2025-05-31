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
            CreateMap<Category, CategoryDTO>();

            CreateMap<CategoryDTO, Category>();

        }

    }
}
