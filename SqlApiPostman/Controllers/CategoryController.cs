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

        /// <summary>
        /// Fetches all categories from the MSSQL CategoryRepo.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching all categories");
            try
            {
                IEnumerable<CategoryDTO> categories = await _categoryRepo.GetAllCategoriesAsync();
                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found.");
                    return NotFound(new { message = "No Categories Found" });
                }
                _logger.LogInformation($"Found {categories.Count()} categories.");
                return Ok(new
                {
                    CategoryCount = $"Found {categories.Count()} categories.",
                    Categories = categories
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching categories.");
                throw new Exception("An error occurred while fetching categories.", ex);
            }
        }

        /// <summary>
        /// Fetches a specific category by ID from the MSSQL CategoryRepo.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                return Ok(new
                {
                    message = $"Category with ID : {id} found successfully.",
                    Category = category
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the category by ID.");
                return StatusCode(404, "Category Not Found");
            }
        }

        /// <summary>
        /// Adds a new category to the MSSQL CategoryRepo.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
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
                if (newCategoryId <= 0)
                {
                    _logger.LogWarning("Failed to add the category.");
                    return BadRequest("Failed to add the category.");
                }
                _logger.LogInformation($"New category added with ID: {newCategoryId}");
                return CreatedAtAction(nameof(GetCategoryById), new { id = newCategoryId }, newCategoryId); // 201 Created
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the category.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates an existing category in the MSSQL CategoryRepo.
        /// </summary>
        /// <param name="categoryDTO"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes a category by ID from the MSSQL CategoryRepo.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid category ID provided for deletion.");
                return BadRequest("Invalid category ID.");
            }
            try
            {
                if(await _categoryRepo.GetCategoryByIdAsync(id) != null)
                {
                    // Assuming the repo has a method to delete by ID
                    bool isDeleted = await _categoryRepo.DeleteCategoryAsync(id);
                    if (!isDeleted)
                    {
                        _logger.LogWarning($"Failed to delete category with ID: {id}");
                        return NotFound($"Failed to delete category with ID: {id}");
                    }
                    _logger.LogInformation($"Category with ID: {id} deleted successfully.");
                    return NoContent();
                }
                _logger.LogWarning($"Category with ID: {id} not found for deletion.");
                return NotFound($"Category with ID: {id} not found for deletion.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the category.");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
