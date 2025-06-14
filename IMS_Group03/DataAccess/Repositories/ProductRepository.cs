using IMS_Group03.DataAccess;
using IMS_Group03.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetProductsWithSuppliersAsync()
        {
            // Ensure _context.Products is not null and ToListAsync() is called.
            // ToListAsync() itself returns an empty list if no items, not null.
            var products = await _context.Products
                                 .Include(p => p.Supplier)
                                 .OrderBy(p => p.Name)
                                 .ToListAsync();
            return products; // This should be List<Product> (empty if no data), not null.
        }

        public async Task<Product?> GetProductWithSupplierAsync(int id) =>
            await _context.Products
                          .Include(p => p.Supplier)
                          .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
        {
            var products = await _context.Products
                                 .Where(p => p.QuantityInStock < threshold)
                                 .ToListAsync();
            return products;
        }

        public async Task<bool> SkuExistsAsync(string sku, int? currentProductId = null)
        {
            if (string.IsNullOrWhiteSpace(sku)) return false;
            if (currentProductId.HasValue)
            {
                return await _context.Products.AnyAsync(p => p.Sku == sku && p.Id != currentProductId.Value);
            }
            return await _context.Products.AnyAsync(p => p.Sku == sku);
        }
    }
}