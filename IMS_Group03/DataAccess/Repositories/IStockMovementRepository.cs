// --- DataAccess/Repositories/IStockMovementRepository.cs ---
using IMS_Group03.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public interface IStockMovementRepository : IRepository<StockMovement>
    {
        Task<IEnumerable<StockMovement>> GetMovementsForProductAsync(int productId);
        Task<IEnumerable<StockMovement>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<StockMovement>> GetMovementsByTypeAsync(MovementType movementType);

        
        Task<IEnumerable<StockMovement>> GetMovementsForPurchaseOrderItemAsync(int purchaseOrderItemId);
    }
}