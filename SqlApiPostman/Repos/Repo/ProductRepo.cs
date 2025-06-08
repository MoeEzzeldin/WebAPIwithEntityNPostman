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
        private readonly ILogger<ProductRepo> _logger;
        private readonly MyAppDbContext _context;
        private readonly IMapper _mapper;

        public ProductRepo(MyAppDbContext context, IMapper mapper, ILogger<ProductRepo> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Fetches all products from the MSSQL database.
        /// </summary>
        /// <returns></returns>
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
            try
            {
                _logger.LogInformation($"Fetching product with ID: {id} from the database.");
                IQueryable<Product> query = _context.Products.Include(p => p.Category).AsNoTracking().Where(p => p.Id == id);
                // Execute the query and get the first or default result
                Product? product = await query.FirstOrDefaultAsync();
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

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> AddProductAsync(ProductDTO productDTO)
        {
            try
            {
                _logger.LogInformation("Adding a new product to the database.");
                Product newProduct = _mapper.Map<Product>(productDTO);
                await _context.Products.AddAsync(newProduct);
                int createdProduct = await _context.SaveChangesAsync();
                if (createdProduct <= 0)
                {
                    _logger.LogWarning("No rows were affected while adding the product.");
                    return 0;
                }
                _logger.LogInformation($"Product with ID: {createdProduct} added successfully.");
                return createdProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new product.");
                throw new Exception("An error occurred while adding the product.", ex);
            }
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="productDTO"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> UpdateProductAsync(ProductDTO productDTO)
        {
            try
            {
                _logger.LogInformation($"Updating product with ID: {productDTO.Id} in the database.");
                Product? existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == productDTO.Id);

                if (existingProduct == null)
                {
                    _logger.LogWarning($"Product with ID: {productDTO.Id} not found for update.");
                    return 0; // or throw an exception if preferred
                }

                // Map the updated properties from DTO to Entity
                _mapper.Map(productDTO, existingProduct);
                int rowsAffected = await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID: {productDTO.Id} updated successfully. Rows affected: {rowsAffected}");
                return rowsAffected > 0 ? productDTO.Id : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product.");
                throw new Exception("An error occurred while updating the product.", ex);
            }
        }

        /// <summary>
        /// Deletes a product from the database by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                Product? productToDelete = await _context.Products.FindAsync(id);
                if (productToDelete == null)
                {
                    _logger.LogWarning($"Product with ID: {id} not found for deletion.");
                    return false; // or throw an exception if preferred
                }
                _context.Products.Remove(productToDelete);
                return await _context.SaveChangesAsync() > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product.");
                throw new Exception("An error occurred while deleting the product.", ex);
            }
        }
    }
}