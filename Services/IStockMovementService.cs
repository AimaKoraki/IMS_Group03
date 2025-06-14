// --- CORRECTED AND FINALIZED: Services/IStockMovementService.cs ---
using IMS_Group03.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMS_Group03.DataAccess.Repositories;

namespace IMS_Group03.Services
{
    public interface IStockMovementService
    {
        // --- Read Operations ---
        Task<IEnumerable<StockMovement>> GetMovementsForProductAsync(int productId);
        Task<IEnumerable<StockMovement>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetCurrentStockLevelAsync(int productId);

        // --- Write Operations (Top-Level: These are complete transactions) ---
        Task RecordStockAdjustmentAsync(int productId, int newActualQuantity, string reason, int? performedByUserId);
        Task RecordSaleAsync(int productId, int quantitySold, string reason, int? performedByUserId); // Example for a sale

        // --- Composable Operation (Lower-Level: Does NOT save, for use by other services) ---
        Task StagePurchaseReceiptAsync(int productId, int quantityReceived, string reason, int? performedByUserId, int? purchaseOrderItemId = null);
    }
}