using IMS_Group03.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMS_Group03.DataAccess.Repositories;
namespace IMS_Group03.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(); // Typically includes supplier info
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetProductWithSupplierAsync(int id); // Note the '?'
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
        Task<bool> IsSkuUniqueAsync(string sku, int? productId = null);
        // Task UpdateProductStockAsync(int productId, int quantityChange); // This might be better handled by StockMovementService
    }
}