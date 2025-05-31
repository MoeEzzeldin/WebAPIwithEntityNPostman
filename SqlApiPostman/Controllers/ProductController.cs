using Microsoft.AspNetCore.Mvc;
using SqlApiPostman.Repos.IRepo;
using SqlApiPostman.Models.Entities;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
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
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            _logger.LogInformation($"Fetching details for product with ID: {id}.");
            ProductDTO product = await _productRepo.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID: {id} not found.");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] ProductDTO product)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product creation");
                return BadRequest(ModelState);
            }
            try
            {
                int result = await _productRepo.AddProductAsync(product);
                _logger.LogInformation($"Created new product with ID: {product.Id}");
                return CreatedAtAction(nameof(GetProduct), new { id = result }, product);
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
            if (productDTO == null || productDTO.Id == null)
            {
                _logger.LogError("Invalid product data for update.");
                return BadRequest("Invalid product data.");
            }

            try
            {
                _logger.LogInformation($"Updating product with ID: {productDTO.Id}.");
                int result = await _productRepo.UpdateProductAsync(productDTO);
                if (result == 0)
                {
                    _logger.LogWarning($"Product with ID: {productDTO.Id} not found for update.");
                    return NotFound();
                }
                _logger.LogInformation($"Product with ID: {result} updated successfully.");

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
                _logger.LogInformation($"Deleting product with ID: {id}.");
                bool result = await _productRepo.DeleteProductAsync(id);
                if (!result)
                {
                    _logger.LogWarning($"Product with ID: {id} passed Id check but was not deleted");
                    return NotFound();
                }
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
