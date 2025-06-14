// File: DataAccess/Repositories/SupplierRepository.cs
using IMS_Group03.DataAccess; // For AppDbContext
using IMS_Group03.Models;     // For Supplier
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Supplier>> GetAllWithProductsAsync()
        {
            return await _context.Suppliers
                                 .Include(s => s.Products)
                                 .OrderBy(s => s.Name)
                                 .ToListAsync();
        }

        public async Task<Supplier> GetByIdWithProductsAsync(int supplierId)
        {
            return await _context.Suppliers
                                 .Include(s => s.Products)
                                 .FirstOrDefaultAsync(s => s.Id == supplierId);
        }

        public async Task<IEnumerable<Supplier>> GetAllWithPurchaseOrdersAsync()
        {
            return await _context.Suppliers
                                 .Include(s => s.PurchaseOrders)
                                 .OrderBy(s => s.Name)
                                 .ToListAsync();
        }

        public async Task<Supplier> GetByIdWithPurchaseOrdersAsync(int supplierId)
        {
            return await _context.Suppliers
                                 .Include(s => s.PurchaseOrders)
                                 .FirstOrDefaultAsync(s => s.Id == supplierId);
        }

        public async Task<bool> SupplierNameExistsAsync(string name, int? currentSupplierId = null)
        {
            if (currentSupplierId.HasValue)
            {
                // Check if name exists for another supplier when editing
                return await _context.Suppliers.AnyAsync(s => s.Name == name && s.Id != currentSupplierId.Value);
            }
            // Check if name exists when adding a new supplier
            return await _context.Suppliers.AnyAsync(s => s.Name == name);
        }
    }
}