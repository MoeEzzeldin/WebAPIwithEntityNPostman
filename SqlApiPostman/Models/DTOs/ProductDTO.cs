namespace SqlApiPostman.Models.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        // Foreign key for Category
        public int CategoryId { get; set; }
        public CategoryDTO Category { get; set; } = new CategoryDTO();
    }
}
