using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlApiPostman.Data;
using SqlApiPostman.Models.Entities;
using SqlApiPostman.Repos.IRepo;
namespace SqlApiPostman.Repos.Repo
{
    public class ProductRepo : IProductRepo
    {
        private readonly MyAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductRepo> _logger;
        public ProductRepo(MyAppDbContext context, IMapper mapper, ILogger<ProductRepo> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            _logger.LogInformation("Fetching all products from the database.");

            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching product with ID: {id} from the database.");
            return await _context.Products.FindAsync(id);
        }
        public async Task AddProductAsync(Product product)
        {
            _logger.LogInformation($"Adding new product: {product.Name} to the database.");
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateProductAsync(Product product)
        {
            _logger.LogInformation($"Updating product with ID: {product.Id} in the database.");
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProductAsync(int id)
        {
            _logger.LogInformation($"Deleting product with ID: {id} from the database.");
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"Product with ID: {id} not found for deletion.");
            }
        }
    }
}
