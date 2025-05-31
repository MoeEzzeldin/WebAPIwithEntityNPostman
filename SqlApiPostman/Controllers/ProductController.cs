using Microsoft.AspNetCore.Mvc;
using SqlApiPostman.Repos.IRepo;
using SqlApiPostman.Models.DTOs;
using AutoMapper;

namespace SqlApiPostman.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepo _productRepo;
        private readonly IMapper _mapper;

        public ProductController(ILogger<ProductController> logger, IProductRepo productRepo, IMapper mapper)
        {
            _logger = logger;
            _productRepo = productRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Fetches all products from MSSQL ProductRepo.
        /// </summary>
        /// <returns>Ok(product.Count() + Products[])</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsAsync()
        {
            _logger.LogInformation("Fetching all products");
            try
            {
                var products = await _productRepo.GetAllProductsAsync();
                if (products == null || !products.Any())
                {
                    _logger.LogWarning("No products found.");
                    return NotFound("No products available.");
                }
                _logger.LogInformation($"Found {products.Count()} products.");
                return Ok(new
                {
                    message = $"Found {products.Count()} products.",
                    products = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// first we make sure the id is provided, then
        /// we try fetching specific product by ID from MSSQL ProductRepo.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> we return NotFound if it comes back null or product with it's id</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid product ID provided.");
                return BadRequest("Invalid product ID.");
            }
            try
            {
                ProductDTO product = await _productRepo.GetProductByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID: {id} not found.");
                    return NotFound();
                }
                return Ok(new
                {
                    message = $"Product with ID: {id} found.",
                    product = product
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the product.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProductAsync([FromBody] ProductDTO product)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product creation");
                return BadRequest(ModelState);
            }
            try
            {
                int newProductId = await _productRepo.AddProductAsync(product);
                product.Id = newProductId;
                _logger.LogInformation($"Created new product with ID: {product.Id}");
                return Created();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new product.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDTO productDTO)
        {
            if (productDTO == null)
            {
                _logger.LogError("Invalid product data for update.");
                return BadRequest("Invalid product data.");
            }

            try
            {
                if(await _productRepo.GetProductByIdAsync(productDTO.Id) == null)
                {
                    _logger.LogWarning($"Product with ID: {productDTO.Id} not found for update.");
                    return NotFound();
                }
                _logger.LogInformation($"Updating product with ID: {productDTO.Id}.");
                int updatedId = await _productRepo.UpdateProductAsync(productDTO);
                if (updatedId == 0)
                {
                    _logger.LogWarning($"Product was found, but not updated");
                    return StatusCode(500, "Found Product, but error with UpdateProductAsync Repo");
                }
                _logger.LogInformation($"Product with ID: {updatedId} updated successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product.");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid product ID for deletion.");
                return BadRequest("Invalid product ID.");
            }
            try
            {
                ProductDTO productTODelete = await _productRepo.GetProductByIdAsync(id);
                if (productTODelete == null)
                {
                    _logger.LogWarning($"Product with ID: {id} not found for deletion.");
                    return NotFound();
                }
                _logger.LogInformation($"Deleting product with ID: {id}.");
                bool deleted = await _productRepo.DeleteProductAsync(productTODelete);
                if (!deleted)
                {
                    _logger.LogWarning($"Product with ID: {id} passed Id check but was not deleted");
                    return StatusCode(500, "Internal server error");
                }
                _logger.LogInformation($"Product with ID: {id} deleted successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
