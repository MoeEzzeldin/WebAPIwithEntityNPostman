using SqlApiPostman.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
namespace SqlApiPostman.Repos.IRepo
{
    public interface IProductRepo
    {
        // Define methods for product repository
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
