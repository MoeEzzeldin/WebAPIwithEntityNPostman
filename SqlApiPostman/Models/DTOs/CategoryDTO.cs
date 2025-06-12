namespace SqlApiPostman.Models.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Navigation property for related products
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
    }
}