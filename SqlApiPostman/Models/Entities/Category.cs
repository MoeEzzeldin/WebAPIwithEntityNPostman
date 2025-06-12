﻿namespace SqlApiPostman.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Navigation property for related products
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
