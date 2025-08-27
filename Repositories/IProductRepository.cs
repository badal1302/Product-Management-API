using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagementApi.Models;

namespace ProductManagementApi.Repositories
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> Products, int TotalCount)> GetProductsAsync(int pageNumber, int pageSize);
    Task<Product?> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}
