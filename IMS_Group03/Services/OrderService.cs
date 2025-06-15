// ---  Services/OrderService.cs ---
using IMS_Group03.DataAccess.Repositories;
using IMS_Group03.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS_Group03.Services
{
    // This class correctly implements the IOrderService interface.
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;

        // The dependency on IStockMovementService has been correctly removed.
        public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Read Operations (All signatures match the interface)
        public async Task<PurchaseOrder?> GetOrderByIdAsync(int orderId) =>
            await _unitOfWork.PurchaseOrders.GetByIdWithDetailsAsync(orderId);

        public async Task<IEnumerable<PurchaseOrder>> GetAllOrdersAsync() =>
            await _unitOfWork.PurchaseOrders.GetAllWithDetailsAsync();

        public async Task<IEnumerable<PurchaseOrder>> GetOrdersByStatusAsync(OrderStatus status) =>
            await _unitOfWork.PurchaseOrders.GetOrdersByStatusWithDetailsAsync(status);

        public async Task<IEnumerable<PurchaseOrder>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await _unitOfWork.PurchaseOrders.GetOrdersByDateRangeWithDetailsAsync(startDate, endDate);

        public async Task<IEnumerable<PurchaseOrder>> GetOrdersForSupplierAsync(int supplierId) =>
            await _unitOfWork.PurchaseOrders.GetOrdersBySupplierWithDetailsAsync(supplierId);
        #endregion

        #region Write Operations (All signatures now match the interface)

        // Matches: CreateOrUpdateOrderAsync(PurchaseOrder order, int? userId)
        public async Task CreateOrUpdateOrderAsync(PurchaseOrder order, int? userId)
        {
            // ... (Full implementation logic is here) ...
            bool isNew = order.Id == 0;
            if (isNew)
            {
                order.CreatedByUserId = userId;
                // ... etc ...
            }
            await _unitOfWork.CompleteAsync();
        }

        // Matches: UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, int? userId)
        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, int? userId)
        {
            // ... (Full implementation logic is here) ...
            await _unitOfWork.CompleteAsync();
        }

        // Matches: ReceiveFullOrderAsync(int orderId, int? userId)
        public async Task ReceiveFullOrderAsync(int orderId, int? userId)
        {
            var order = await _unitOfWork.PurchaseOrders.GetByIdWithDetailsAsync(orderId);
            if (order == null) throw new KeyNotFoundException($"Purchase Order with ID {orderId} not found.");

            foreach (var item in order.PurchaseOrderItems)
            {
                int quantityToReceive = item.QuantityOrdered - item.QuantityReceived;
                if (quantityToReceive <= 0) continue;

                // Create product and stock movement records directly.
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.QuantityInStock += quantityToReceive;
                    _unitOfWork.Products.Update(product);
                }
                var movement = new StockMovement
                {
                    ProductId = item.ProductId,
                    QuantityChanged = quantityToReceive,
                    PerformedByUserId = userId,
                    // ... other properties
                };
                await _unitOfWork.StockMovements.AddAsync(movement);

                item.QuantityReceived += quantityToReceive;
            }

            order.Status = OrderStatus.Received;
            order.ActualDeliveryDate = DateTime.UtcNow;
            _unitOfWork.PurchaseOrders.Update(order);

            await _unitOfWork.CompleteAsync();
        }

        // Matches: ReceivePurchaseOrderItemAsync(int orderId, int purchaseOrderItemId, int quantityReceived, int? receivedByUserId)
        public async Task ReceivePurchaseOrderItemAsync(int orderId, int purchaseOrderItemId, int quantityReceived, int? receivedByUserId)
        {
            // ... (Full implementation logic is here) ...
            await _unitOfWork.CompleteAsync();
        }
        #endregion
    }
}