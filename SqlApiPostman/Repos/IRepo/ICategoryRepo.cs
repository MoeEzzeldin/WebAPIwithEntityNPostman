using SqlApiPostman.Models.Entities;
using SqlApiPostman.Models.DTOs;
namespace SqlApiPostman.Repos.IRepo
{
    public interface ICategoryRepo
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<int> AddCategoryAsync(CategoryDTO category);
        Task<int> UpdateCategoryAsync(CategoryDTO category);
        Task<bool> DeleteCategoryAsync(int id);

    }
}
