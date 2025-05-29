using SqlApiPostman.Models.Entities;

namespace SqlApiPostman.Repos.IRepo
{
    public interface ICategoryRepo
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(Guid id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Guid id);
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
    }
}
