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
            try
            {
                var product = await _context.Products.ToListAsync();
                if (product == null || !product.Any())
                {
                    _logger.LogWarning("No products found in the database.");
                    return Enumerable.Empty<ProductDTO>();
                }
                return _mapper.Map<IEnumerable<ProductDTO>>(product);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all products.");
                throw new Exception("An error occurred while fetching all products.", ex);
            }
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Attempted to fetch a product with an invalid ID.");
                throw new ArgumentException("ID must be greater than zero", nameof(id));
            }
            try
            {
                _logger.LogInformation($"Fetching product with ID: {id} from the database.");
                var myProduct = await _context.Products.FindAsync(id);
                return _mapper.Map<ProductDTO>(myProduct);
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
                if (!await _context.Products.AnyAsync(p => p.Id == productDTO.Id))
                {
                    _logger.LogWarning($"Product with ID: {productDTO.Id} not found for update.");
                    return 0;
                }

                var productEntity = _mapper.Map<Product>(productDTO);
                _context.Products.Update(productEntity);
                int rowsAffected = await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID: {productDTO.Id} updated successfully. Rows affected: {rowsAffected}");
                return rowsAffected > 0 ? productDTO.Id : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product.");
                throw;
            }
        }



        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting product with ID: {id} from the database.");
                var product = await _context.Products.FindAsync(id);
                bool productExist = product != null;
                if (productExist)
                {
                    _context.Products.Remove(product);
                    return productExist = await _context.SaveChangesAsync() > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product.");
                throw;
            }
        }
    }
}
