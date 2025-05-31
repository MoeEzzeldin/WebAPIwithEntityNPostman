using SqlApiPostman.Models.Entities;
using SqlApiPostman.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
namespace SqlApiPostman.Repos.IRepo
{
    public interface IProductRepo
    {
        // Define methods for product repository
        Task <IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task <ProductDTO> GetProductByIdAsync(int id);
        Task <int> AddProductAsync(ProductDTO product);
        Task <int> UpdateProductAsync(ProductDTO product);
        Task <bool> DeleteProductAsync(ProductDTO product);
    }
}
