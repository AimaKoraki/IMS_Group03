using IMS_Group03.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithSuppliersAsync();
        Task<Product?> GetProductWithSupplierAsync(int id);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
        Task<bool> SkuExistsAsync(string sku, int? currentProductId = null);
    }
}