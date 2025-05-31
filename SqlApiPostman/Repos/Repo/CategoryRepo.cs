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
        private readonly MyAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepo> _logger;

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
                var categories = await _context.Categories.ToListAsync();
                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found in the database.");
                    return Enumerable.Empty<CategoryDTO>();
                }
                _logger.LogInformation($"Found {categories.Count} categories.");
                return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all categories.");
                throw new Exception("An error occurred while fetching all categories.", ex);
            }
        }
        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Attempted to fetch a category with an invalid ID.");
                throw new ArgumentException("ID must be greater than zero", nameof(id));
            }
            try
            {
                _logger.LogInformation($"Fetching category with ID: {id} from the database.");
                CategoryDTO category = _mapper.Map<CategoryDTO>(await _context.Categories.FindAsync(id));
                if (category == null)
                {
                    _logger.LogWarning($"Category with ID: {id} not found.");
                    return null;
                }
                _logger.LogInformation($"Category with ID: {id} found.");
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the category by ID.");
                throw;
            }
        }

        public async Task<int> AddCategoryAsync(CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                _logger.LogError("Attempted to add a null category.");
                throw new ArgumentNullException(nameof(categoryDto), "Category cannot be null");
            }
            try
            {
                _logger.LogInformation("Adding a new category to the database.");
                Category category = _mapper.Map<Category>(categoryDto);
                _context.Categories.Add(category);
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

        public async Task<int> UpdateCategoryAsync(CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                _logger.LogError("Attempted to update a null category.");
                throw new ArgumentNullException(nameof(categoryDto), "Category cannot be null");
            }
            try
            {
                _logger.LogInformation($"Updating category with ID: {categoryDto.Id} in the database.");

                // Fetch the existing entity
                var existingCategory = await _context.Categories
                    .Include(c => c.Products) // Include navigation if needed
                    .FirstOrDefaultAsync(c => c.Id == categoryDto.Id);

                if (existingCategory == null)
                {
                    _logger.LogWarning($"Category with ID: {categoryDto.Id} not found for update.");
                    return 0;
                }

                // Map updated fields from DTO to entity
                _mapper.Map(categoryDto, existingCategory);

                int rowsAffected = await _context.SaveChangesAsync();
                _logger.LogInformation($"Category with ID: {categoryDto.Id} updated successfully.");
                return rowsAffected > 0 ? existingCategory.Id : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the category.");
                throw new Exception("An error occurred while updating the category.", ex);
            }
        }


    }
}
