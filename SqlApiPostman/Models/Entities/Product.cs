namespace SqlApiPostman.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        // Foreign key for Category
        public int CategoryId { get; set; }
        public Category Category { get; set; } = new Category();

    }
}
