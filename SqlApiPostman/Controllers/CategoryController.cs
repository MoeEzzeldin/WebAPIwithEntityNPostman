using Microsoft.AspNetCore.Mvc;
using SqlApiPostman.Repos.IRepo;
using SqlApiPostman.Models.DTOs;
using SqlApiPostman.Models.Entities;
using AutoMapper;

namespace SqlApiPostman.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryController(ILogger<CategoryController> logger, ICategoryRepo categoryRepo, IMapper mapper)
        {
            _logger = logger;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching all categories");
            try
            {
                IEnumerable<CategoryDTO> categories = await _categoryRepo.GetAllCategoriesAsync();
                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found.");
                    return Enumerable.Empty<CategoryDTO>();
                }
                _logger.LogInformation($"Found {categories.Count()} categories.");
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching categories.");
                throw new Exception("An error occurred while fetching categories.", ex);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryById(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid category ID provided.");
                return BadRequest("Invalid category ID.");
            }
            try
            {
                _logger.LogInformation($"Fetching category with ID: {id}");
                CategoryDTO category = await _categoryRepo.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning($"Category with ID: {id} not found.");
                    return NotFound($"Category with ID: {id} not found.");
                }
                _logger.LogInformation($"Found category with ID: {id}");
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the category by ID.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<ActionResult<int>> AddCategoryAsync([FromBody] CategoryDTO category)
        {
            if (category == null)
            {
                _logger.LogError("Category data is null.");
                return BadRequest("Category data cannot be null.");
            }
            try
            {
                _logger.LogInformation("Adding a new category.");
                int newCategoryId = await _categoryRepo.AddCategoryAsync(category);
                _logger.LogInformation($"New category added with ID: {newCategoryId}");
                return CreatedAtAction(nameof(GetCategoryById), new { id = newCategoryId }, newCategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the category.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut]
        public async Task<ActionResult> UpdateCategoryAsync([FromBody] CategoryDTO categoryDTO)
        {
            if (categoryDTO == null || categoryDTO.Id <= 0)
            {
                _logger.LogError("Invalid category data provided.");
                return BadRequest("Invalid category data.");
            }
            try
            {
                CategoryDTO categoryExist = await _categoryRepo.GetCategoryByIdAsync(categoryDTO.Id);
                if (categoryExist == null)
                {
                    _logger.LogWarning($"Category with ID: {categoryDTO.Id} not found for update.");
                    return NotFound($"Category with ID: {categoryDTO.Id} not found.");
                }
                _logger.LogInformation($"Updating category with ID: {categoryDTO.Id}");
                int updatedCategoryId = await _categoryRepo.UpdateCategoryAsync(categoryDTO);
                if (updatedCategoryId <= 0)
                {
                    _logger.LogWarning($"No category was updated with ID: {categoryDTO.Id}");
                    return NotFound($"No category was updated with ID: {categoryDTO.Id}");
                }
                _logger.LogInformation($"Category with ID: {categoryDTO.Id} updated successfully.");
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the category.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
