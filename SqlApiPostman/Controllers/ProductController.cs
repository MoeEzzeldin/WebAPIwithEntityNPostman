using Microsoft.AspNetCore.Mvc;
using SqlApiPostman.Repos.IRepo;
using SqlApiPostman.Models.Entities;

namespace SqlApiPostman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase  // Change to ControllerBase for API
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepo _productRepo;

        public ProductController(ILogger<ProductController> logger, IProductRepo productRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            _logger.LogInformation("Fetching all products");
            var products = await _productRepo.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            _logger.LogInformation($"Fetching details for product with ID: {id}.");
            var product = await _productRepo.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID: {id} not found.");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            _logger.LogInformation($"Attempting to create new product: {product.Name}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product creation");
                return BadRequest(ModelState);
            }

            await _productRepo.AddProductAsync(product);
            _logger.LogInformation($"Created new product with ID: {product.Id}");

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                _logger.LogWarning($"Product ID mismatch: {id} does not match {product.Id}.");
                return BadRequest("ID in URL doesn't match product ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Updating product with ID: {id}.");
            await _productRepo.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation($"Deleting product with ID: {id}.");
            await _productRepo.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
