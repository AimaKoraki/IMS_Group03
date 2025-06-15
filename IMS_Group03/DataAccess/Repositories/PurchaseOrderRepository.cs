// DataAccess/Repositories/PurchaseOrderRepository.cs ---
using IMS_Group03.DataAccess;
using IMS_Group03.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(AppDbContext context) : base(context)
        {
        }

        // This private helper method is excellent and needs no changes.
        private IQueryable<PurchaseOrder> GetPurchaseOrdersWithDetailsQuery()
        {
            return _context.PurchaseOrders
                           .Include(po => po.Supplier)
                           .Include(po => po.PurchaseOrderItems)
                               .ThenInclude(poi => poi.Product);
        }

        
        public async Task<PurchaseOrder?> GetByIdWithDetailsAsync(int purchaseOrderId)
        {
            // The FirstOrDefaultAsync logic was already correct.
            return await GetPurchaseOrdersWithDetailsQuery()
                           .FirstOrDefaultAsync(po => po.Id == purchaseOrderId);
        }

        // --- All other methods are already correct and need no changes. ---

        public async Task<IEnumerable<PurchaseOrder>> GetAllWithDetailsAsync()
        {
            return await GetPurchaseOrdersWithDetailsQuery()
                           .OrderByDescending(po => po.OrderDate)
                           .ThenByDescending(po => po.Id)
                           .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetOrdersByDateRangeWithDetailsAsync(DateTime startDate, DateTime endDate)
        {
            DateTime endOfDay = endDate.Date.AddDays(1).AddTicks(-1);
            return await GetPurchaseOrdersWithDetailsQuery()
                           .Where(po => po.OrderDate >= startDate.Date && po.OrderDate <= endOfDay)
                           .OrderByDescending(po => po.OrderDate)
                           .ThenByDescending(po => po.Id)
                           .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetOrdersByStatusWithDetailsAsync(OrderStatus status)
        {
            return await GetPurchaseOrdersWithDetailsQuery()
                           .Where(po => po.Status == status)
                           .OrderByDescending(po => po.OrderDate)
                           .ThenByDescending(po => po.Id)
                           .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetOrdersBySupplierWithDetailsAsync(int supplierId)
        {
            return await GetPurchaseOrdersWithDetailsQuery()
                           .Where(po => po.SupplierId == supplierId)
                           .OrderByDescending(po => po.OrderDate)
                           .ThenByDescending(po => po.Id)
                           .ToListAsync();
        }
        public async Task<IEnumerable<PurchaseOrder>> GetOrdersContainingProductAsync(int productId)
        {
            return await _context.PurchaseOrders
                           .Include(po => po.Supplier)
                           .Where(po => po.PurchaseOrderItems.Any(poi => poi.ProductId == productId))
                           .OrderByDescending(po => po.OrderDate)
                           .ToListAsync();
        }
    }
}