using SqlApiPostman.Repos.IRepo;
using SqlApiPostman.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SqlApiPostman.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;


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

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching all categories from the database.");
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            _logger.LogInformation($"Fetching category with ID: {id} from the database.");
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            _logger.LogInformation($"Adding new category: {category.Name} to the database.");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _logger.LogInformation($"Updating category with ID: {category.Id} in the database.");
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            _logger.LogInformation($"Deleting category with ID: {id} from the database.");
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"Category with ID: {id} not found for deletion.");
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync()
        {
            _logger.LogInformation("Fetching categories with their related products from the database.");
            return await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();
        }
    }
}
