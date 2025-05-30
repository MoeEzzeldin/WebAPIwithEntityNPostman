using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlApiPostman.Data;
using SqlApiPostman.Models.DTOs;
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
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            _logger.LogInformation("Fetching all products from the database.");

            var product = await _context.Products.ToListAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(product);

        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            ProductDTO product = null;
            if (id <= 0)
            {
                _logger.LogError("Attempted to fetch a product with an invalid ID.");
                throw new ArgumentException("ID must be greater than zero", nameof(id));
            }
            try
            {
                _logger.LogInformation($"Fetching product with ID: {id} from the database.");
                var myProduct = await _context.Products.FindAsync(id);
                _mapper.Map(myProduct, product);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the product by ID.");
                throw;
            }
        }

        public async Task<int> AddProductAsync(ProductDTO product)
        {
            if (product == null)
            {
                _logger.LogError("Attempted to add a null product.");
                throw new ArgumentNullException(nameof(product), "Product cannot be null");
            }
            try
            {
                var newProduct = _mapper.Map<Product>(product);
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product with ID: {newProduct.Id} added successfully.");
                return newProduct.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new product.");
                throw;
            }
        }

        public async Task<int> UpdateProductAsync(ProductDTO productDTO)
        {
            if (productDTO == null)
            {
                _logger.LogError("Attempted to update a null product.");
                throw new ArgumentNullException(nameof(productDTO), "Product cannot be null");
            }
            try
            {
                var existingProduct = await _context.Products.FindAsync(productDTO.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning($"Product with ID: {productDTO.Id} not found for update.");
                    throw new KeyNotFoundException($"Product with ID: {productDTO.Id} not found.");
                }
                // Update properties
                _mapper.Map(productDTO, existingProduct);
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product with ID: {productDTO.Id} updated successfully.");

                return existingProduct.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product.");

            }

            return 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Attempted to delete a product with an invalid ID.");
                throw new ArgumentException("ID must be greater than zero", nameof(id));
            }
            try
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
                return product != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product.");
                return false;
            }
        }
    }
}
