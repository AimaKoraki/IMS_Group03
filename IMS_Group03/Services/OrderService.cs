// --- CORRECTED AND FINALIZED: Services/OrderService.cs ---
using IMS_Group03.DataAccess.Repositories;
using IMS_Group03.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS_Group03.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockMovementService _stockMovementService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IStockMovementService stockMovementService, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _stockMovementService = stockMovementService;
            _logger = logger;
        }

        #region Read Operations
        public async Task<PurchaseOrder?> GetOrderByIdAsync(int orderId) =>
            await _unitOfWork.PurchaseOrders.GetByIdWithDetailsAsync(orderId);

        public async Task<IEnumerable<PurchaseOrder>> GetAllOrdersAsync() =>
            await _unitOfWork.PurchaseOrders.GetAllWithDetailsAsync();

        public async Task<IEnumerable<PurchaseOrder>> GetOrdersByStatusAsync(OrderStatus status) =>
            await _unitOfWork.PurchaseOrders.GetOrdersByStatusWithDetailsAsync(status);

        public async Task<IEnumerable<PurchaseOrder>> GetOrdersForSupplierAsync(int supplierId) =>
            await _unitOfWork.PurchaseOrders.GetOrdersBySupplierWithDetailsAsync(supplierId);
        #endregion

        #region Write Operations
        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder order, IEnumerable<PurchaseOrderItem> items, int createdByUserId)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (items == null || !items.Any()) throw new ArgumentException("Order must contain at least one item.", nameof(items));

            order.Status = OrderStatus.Pending;
            order.OrderDate = DateTime.UtcNow;
            order.DateCreated = DateTime.UtcNow;
            order.LastUpdated = DateTime.UtcNow;
            order.CreatedByUserId = createdByUserId;

            foreach (var item in items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null) throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");
                if (item.UnitPrice <= 0) item.UnitPrice = product.Price;
            }
            order.PurchaseOrderItems = items.ToList();

            await _unitOfWork.PurchaseOrders.AddAsync(order);
            await _unitOfWork.CompleteAsync(); // Commit transaction

            _logger.LogInformation("Successfully created Purchase Order {OrderId} for Supplier {SupplierId} by User {UserId}.", order.Id, order.SupplierId, createdByUserId);
            return order;
        }

        public async Task UpdatePurchaseOrderAsync(PurchaseOrder orderHeader, IEnumerable<PurchaseOrderItem> updatedItems, int updatedByUserId)
        {
            var existingOrder = await _unitOfWork.PurchaseOrders.GetByIdWithDetailsAsync(orderHeader.Id);
            if (existingOrder == null) throw new KeyNotFoundException($"Purchase Order with ID {orderHeader.Id} not found.");
            if (existingOrder.Status != OrderStatus.Pending && existingOrder.Status != OrderStatus.Processing)
            {
                throw new InvalidOperationException($"Cannot update order in status '{existingOrder.Status}'.");
            }

            // Update header
            existingOrder.SupplierId = orderHeader.SupplierId;
            existingOrder.ExpectedDeliveryDate = orderHeader.ExpectedDeliveryDate;
            existingOrder.Notes = orderHeader.Notes;
            existingOrder.LastUpdated = DateTime.UtcNow;

            // Manage line items
            var updatedItemsLookup = updatedItems.ToDictionary(i => i.ProductId);
            var existingItems = existingOrder.PurchaseOrderItems.ToList();

            // Delete items not in the new list
            foreach (var dbItem in existingItems.Where(db => !updatedItemsLookup.ContainsKey(db.ProductId)))
            {
                existingOrder.PurchaseOrderItems.Remove(dbItem);
            }

            // Add new or update existing items
            foreach (var updatedItem in updatedItems)
            {
                var dbItem = existingOrder.PurchaseOrderItems.FirstOrDefault(i => i.ProductId == updatedItem.ProductId);
                if (dbItem != null) // Update existing
                {
                    dbItem.QuantityOrdered = updatedItem.QuantityOrdered;
                    dbItem.UnitPrice = updatedItem.UnitPrice;
                }
                else // Add new
                {
                    existingOrder.PurchaseOrderItems.Add(updatedItem);
                }
            }

            _unitOfWork.PurchaseOrders.Update(existingOrder);
            await _unitOfWork.CompleteAsync(); // Commit transaction

            _logger.LogInformation("Successfully updated Purchase Order {OrderId} by User {UserId}.", existingOrder.Id, updatedByUserId);
        }

        public async Task CancelPurchaseOrderAsync(int orderId, string reason, int cancelledByUserId)
        {
            var order = await _unitOfWork.PurchaseOrders.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException($"Purchase Order with ID {orderId} not found.");
            if (order.Status == OrderStatus.Received || order.Status == OrderStatus.Shipped)
            {
                throw new InvalidOperationException($"Cannot cancel an order that is already '{order.Status}'.");
            }

            order.Status = OrderStatus.Cancelled;
            order.Notes = $"Cancelled by User {cancelledByUserId}. Reason: {reason} - {order.Notes}";
            order.LastUpdated = DateTime.UtcNow;

            _unitOfWork.PurchaseOrders.Update(order);
            await _unitOfWork.CompleteAsync(); // Commit transaction

            _logger.LogWarning("Cancelled Purchase Order {OrderId} by User {UserId}. Reason: {Reason}", orderId, cancelledByUserId, reason);
        }

        public async Task ReceivePurchaseOrderItemAsync(int orderId, int productIdOfItemToReceive, int quantityReceived, int receivedByUserId)
        {
            if (quantityReceived <= 0) throw new ArgumentException("Quantity received must be positive.");

            var order = await _unitOfWork.PurchaseOrders.GetByIdWithDetailsAsync(orderId);
            if (order == null) throw new KeyNotFoundException($"Purchase Order with ID {orderId} not found.");

            var item = order.PurchaseOrderItems.FirstOrDefault(i => i.ProductId == productIdOfItemToReceive);
            if (item == null) throw new KeyNotFoundException($"Product ID {productIdOfItemToReceive} not found in Order {orderId}.");

            // Stage the stock movement but DO NOT SAVE
            string reason = $"Received for PO #{orderId}";
            await _stockMovementService.StagePurchaseReceiptAsync(item.ProductId, quantityReceived, reason, receivedByUserId, item.PurchaseOrderId);

            // Update the PO item and order status
            item.QuantityReceived += quantityReceived;
            order.Status = order.PurchaseOrderItems.All(i => i.QuantityReceived >= i.QuantityOrdered)
                ? OrderStatus.Received
                : OrderStatus.Processing;
            order.LastUpdated = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync(); // Commit transaction (updates stock, PO item, and PO header together)
            _logger.LogInformation("Received {Quantity} of Product {ProductId} for PO {OrderId} by User {UserId}.", quantityReceived, productIdOfItemToReceive, orderId, receivedByUserId);
        }

        public async Task ReceiveFullPurchaseOrderAsync(int orderId, int receivedByUserId)
        {
            var order = await _unitOfWork.PurchaseOrders.GetByIdWithDetailsAsync(orderId);
            if (order == null) throw new KeyNotFoundException($"Purchase Order with ID {orderId} not found.");

            foreach (var item in order.PurchaseOrderItems)
            {
                int quantityToReceive = item.QuantityOrdered - item.QuantityReceived;
                if (quantityToReceive <= 0) continue;

                string reason = $"Full receipt for PO #{orderId}";
                await _stockMovementService.StagePurchaseReceiptAsync(item.ProductId, quantityToReceive, reason, receivedByUserId, item.PurchaseOrderId);
                item.QuantityReceived += quantityToReceive;
            }

            order.Status = OrderStatus.Received;
            order.ActualDeliveryDate = DateTime.UtcNow;
            order.LastUpdated = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync(); // Commit all staged changes in one transaction
            _logger.LogInformation("Full receipt for PO {OrderId} processed by User {UserId}.", orderId, receivedByUserId);
        }
        #endregion
    }
}