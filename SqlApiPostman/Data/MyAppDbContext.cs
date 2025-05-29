using Microsoft.EntityFrameworkCore;
using SqlApiPostman.Models.Entities;

namespace SqlApiPostman.Data
{
    public class MyAppDbContext : DbContext
    {
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
        {
        }

        // Add DbSets for your entities
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Apply configurations here if needed
        }
    }
}
