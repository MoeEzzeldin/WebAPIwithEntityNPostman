using SqlApiPostman.Models.Entities;
using SqlApiPostman.Models.DTOs;
using AutoMapper;
using SqlApiPostman.Data;
using Microsoft.EntityFrameworkCore;
using SqlApiPostman.Repos.IRepo;

namespace SqlApiPostman.Repos.Repo
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly ILogger<CategoryRepo> _logger;
        private readonly MyAppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryRepo(MyAppDbContext context, IMapper mapper, ILogger<CategoryRepo> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Fetches all categories from the MSSQL database.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all categories from the database.");
                IQueryable<Category> query = _context.Categories.Include(p => p.Products).AsNoTracking(); ;
                return _mapper.Map<IEnumerable<CategoryDTO>>(await query.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all categories.");
                return Enumerable.Empty<CategoryDTO>();
            }
        }


        /// <summary>
        /// Queries the database for a specific category by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching category with ID: {id} from the database.");

                IQueryable<Category> query = _context.Categories.AsNoTracking().Where(c => c.Id == id); ;

                Category? category = await query.FirstOrDefaultAsync();

                if (category == null || category.Id == 0)
                {
                    _logger.LogWarning($"Category with ID: {id} not found.");
                    return new CategoryDTO();
                }
                _logger.LogInformation($"Category with ID: {id} found.");
                return _mapper.Map<CategoryDTO>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the category by ID.");
                throw;
            }
        }

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<int> AddCategoryAsync(CategoryDTO categoryDTO)
        {
            try
            {
                _logger.LogInformation("Adding a new category to the database.");
                Category newCategory = _mapper.Map<Category>(categoryDTO);
                await _context.Categories.AddAsync(newCategory);
                int createdCategory = await _context.SaveChangesAsync();
                if (createdCategory <= 0)
                {
                    _logger.LogWarning("No category was added to the database.");
                    return 0;
                }
                _logger.LogInformation($"Category with ID: {createdCategory} added successfully.");
                return createdCategory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new category.");
                throw new Exception("An error occurred while adding a new category.", ex);
            }
        }

        /// <summary>
        /// Updates an existing category in the database.
        /// </summary>
        /// <param name="categoryDTO"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<int> UpdateCategoryAsync(CategoryDTO categoryDTO)
        {
            try
            {
                _logger.LogInformation($"Updating category with ID: {categoryDTO.Id} in the database.");

                Category? existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == categoryDTO.Id);

                if (existingCategory == null)
                {
                    _logger.LogWarning($"Category with ID: {categoryDTO.Id} not found for update.");
                    return 0;
                }

                _mapper.Map(categoryDTO, existingCategory);
                int rowsAffected = await _context.SaveChangesAsync();

                _logger.LogInformation($"Category with ID: {categoryDTO.Id} updated successfully.");
                return rowsAffected > 0 ? existingCategory.Id : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the category.");
                throw new Exception("An error occurred while updating the category.", ex);
            }
        }

        /// <summary>
        /// Deletes a category from the database by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                Category? categoryToDelete = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (categoryToDelete == null)
                {
                    _logger.LogWarning($"Category with ID: {id} not found for deletion.");
                    return false;
                }
                _context.Categories.Remove(categoryToDelete);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the category with ID: {id}.");
                throw new Exception($"An error occurred while deleting the category with ID: {id}.", ex);
            }
        }
    }
}