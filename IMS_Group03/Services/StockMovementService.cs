// --- CORRECTED AND FINALIZED: Services/StockMovementService.cs ---
using IMS_Group03.DataAccess.Repositories;
using IMS_Group03.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS_Group03.Services
{
    public class StockMovementService : IStockMovementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StockMovementService> _logger;

        public StockMovementService(IUnitOfWork unitOfWork, ILogger<StockMovementService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // --- Read Operations ---
        public async Task<IEnumerable<StockMovement>> GetMovementsForProductAsync(int productId)
        {
            return await _unitOfWork.StockMovements.GetMovementsForProductAsync(productId);
        }

        public async Task<IEnumerable<StockMovement>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.StockMovements.GetMovementsByDateRangeAsync(startDate, endDate);
        }

        public async Task<int> GetCurrentStockLevelAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException($"Product with ID {productId} not found.");
            return product.QuantityInStock;
        }

        // --- TOP-LEVEL WRITE OPERATION ---
        // This is a complete user action (e.g., "User adjusts stock from a form").
        // Therefore, it IS responsible for calling CompleteAsync.
        public async Task RecordStockAdjustmentAsync(int productId, int newActualQuantity, string reason, int? performedByUserId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException($"Product with ID {productId} not found.");

            int quantityChange = newActualQuantity - product.QuantityInStock;
            if (quantityChange == 0) return; // No change needed

            MovementType type = quantityChange > 0 ? MovementType.Adjustment_In : MovementType.Adjustment_Out;

            await StageStockMovementAsync(product, quantityChange, type, reason, performedByUserId);

            // This is a self-contained operation, so we commit the changes.
            await _unitOfWork.CompleteAsync();
        }

        // Another example of a top-level operation.
        public async Task RecordSaleAsync(int productId, int quantitySold, string reason, int? performedByUserId)
        {
            if (quantitySold <= 0) throw new ArgumentException("Quantity sold must be positive.");

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException($"Product with ID {productId} not found.");

            await StageStockMovementAsync(product, -quantitySold, MovementType.SaleShipment, reason, performedByUserId);

            await _unitOfWork.CompleteAsync();
        }


        // --- LOWER-LEVEL COMPOSABLE OPERATION ---
        // This method is designed to be called by another service (like OrderService).
        // Therefore, it MUST NOT call CompleteAsync. It only prepares the changes.
        public async Task StagePurchaseReceiptAsync(int productId, int quantityReceived, string reason, int? performedByUserId, int? purchaseOrderItemId = null)
        {
            if (quantityReceived <= 0) throw new ArgumentException("Quantity received must be positive.");

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException($"Product with ID {productId} not found.");

            await StageStockMovementAsync(product, quantityReceived, MovementType.PurchaseReceipt, reason, performedByUserId, purchaseOrderItemId);
        }


        // --- PRIVATE HELPER ---
        // A single, private helper to create and stage movements without saving.
        private async Task StageStockMovementAsync(Product product, int quantityChange, MovementType type, string reason, int? userId, int? poItemId = null)
        {
            // Update the product's stock level in memory
            product.QuantityInStock += quantityChange;
            _unitOfWork.Products.Update(product);

            // Create the log record
            var movement = new StockMovement
            {
                ProductId = product.Id,
                QuantityChanged = quantityChange,
                Type = type,
                Reason = reason,
                MovementDate = DateTime.UtcNow,
                PerformedByUserId = userId,
                PurchaseOrderItemId = poItemId
            };
            await _unitOfWork.StockMovements.AddAsync(movement);
        }
    }
}