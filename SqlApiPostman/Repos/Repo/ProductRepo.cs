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
            try
            {
                _logger.LogInformation("Fetching all products from the database.");
                // Build the query to fetch all products, including their categories
                IQueryable<Product> query = _context.Products.Include(p => p.Category).AsNoTracking(); ;
                // Execute the query and map the results to DTOs
                return _mapper.Map<IEnumerable<ProductDTO>>(await query.ToListAsync());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all products.");
                return Enumerable.Empty<ProductDTO>();
            }
        }


        /// <summary>
        /// Queries the database for a specific product by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Attempted to fetch a product with an invalid ID.");
                throw new KeyNotFoundException($"Entity with ID {id} not found.");
            }
            try
            {
                _logger.LogInformation($"Fetching product with ID: {id} from the database.");
                IQueryable<Product> query = _context.Products.Include(p => p.Category).AsNoTracking().Where(p => p.Id == id);
                // Execute the query and get the first or default result
                Product product = await query.FirstOrDefaultAsync();
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID: {id} not found.");
                    return new ProductDTO();
                }
                return _mapper.Map<ProductDTO>(product);
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
            if (productDTO == null || productDTO.Id <= 0)
            {
                _logger.LogError("Attempted to update a null product.");
                throw new ArgumentNullException(nameof(productDTO), "Product or Product Id cannot be null");
            }
            try
            {
                _logger.LogInformation($"Updating product with ID: {productDTO.Id} in the database.");
                Product existingProduct = await _context.Products.FindAsync(productDTO.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning($"Product with ID: {productDTO.Id} not found for update.");
                    return 0; // or throw an exception if preferred
                }
                // Map the updated properties from productDTO to existingProduct
                _mapper.Map(productDTO, existingProduct);
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
                if (id <= 0)
                {
                    _logger.LogError("Attempted to delete a product with an invalid ID.");
                    throw new ArgumentException("ID must be greater than zero", nameof(id));
                }
                Product productToDelete = await _context.Products.FindAsync(id);
                _context.Products.Remove(productToDelete);
                return await _context.SaveChangesAsync() > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product.");
                throw;
            }
        }
    }
}
