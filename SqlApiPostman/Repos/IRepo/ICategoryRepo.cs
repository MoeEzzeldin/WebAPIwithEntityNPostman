using SqlApiPostman.Models.Entities;
using SqlApiPostman.Models.DTOs;
namespace SqlApiPostman.Repos.IRepo
{
    public interface ICategoryRepo
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(Guid id);
        Task AddCategoryAsync(CategoryDTO category);
        Task UpdateCategoryAsync(CategoryDTO category);
        Task DeleteCategoryAsync(Guid id);
        Task<IEnumerable<CategoryDTO>> GetCategoriesWithProductsAsync();
    }
}
