// ---DataAccess/Repositories/StockMovementRepository.cs ---
using IMS_Group03.DataAccess;
using IMS_Group03.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public class StockMovementRepository : Repository<StockMovement>, IStockMovementRepository
    {
        public StockMovementRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<StockMovement>> GetMovementsForProductAsync(int productId)
        {
            return await _context.StockMovements
                                 .Include(sm => sm.Product)
                                 .Where(sm => sm.ProductId == productId)
                                 .OrderByDescending(sm => sm.MovementDate)
                                 .ThenByDescending(sm => sm.Id)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<StockMovement>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            DateTime endOfDay = endDate.Date.AddDays(1).AddTicks(-1);
            return await _context.StockMovements
                                 .Include(sm => sm.Product)
                                 .Where(sm => sm.MovementDate >= startDate.Date && sm.MovementDate <= endOfDay)
                                 .OrderByDescending(sm => sm.MovementDate)
                                 .ThenByDescending(sm => sm.Id)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<StockMovement>> GetMovementsByTypeAsync(MovementType movementType)
        {
            return await _context.StockMovements
                                 .Include(sm => sm.Product)
                                 .Where(sm => sm.Type == movementType)
                                 .OrderByDescending(sm => sm.MovementDate)
                                 .ThenByDescending(sm => sm.Id)
                                 .ToListAsync();
        }

        // This implementation is now correct because the model has the required properties
        public async Task<IEnumerable<StockMovement>> GetMovementsForPurchaseOrderItemAsync(int purchaseOrderItemId)
        {
            return await _context.StockMovements
                                 .Include(sm => sm.Product)
                                 .Include(sm => sm.PurchaseOrderItem) // This now works
                                     .ThenInclude(poi => poi.PurchaseOrder)
                                 .Where(sm => sm.PurchaseOrderItemId == purchaseOrderItemId) // This now works
                                 .OrderByDescending(sm => sm.MovementDate)
                                 .ThenByDescending(sm => sm.Id)
                                 .ToListAsync();
        }
    }
}