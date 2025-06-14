// --- CORRECTED AND FINALIZED: Services/IOrderService.cs ---
using IMS_Group03.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMS_Group03.DataAccess.Repositories;
namespace IMS_Group03.Services
{
    public interface IOrderService
    {
        // --- Read Operations ---
        Task<PurchaseOrder?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<PurchaseOrder>> GetAllOrdersAsync();
        Task<IEnumerable<PurchaseOrder>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<PurchaseOrder>> GetOrdersForSupplierAsync(int supplierId);

        // --- Write Operations (Full Transactions) ---

        /// <summary>
        /// Creates a new purchase order and its associated line items.
        /// </summary>
        Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder order, IEnumerable<PurchaseOrderItem> items, int createdByUserId);

        /// <summary>
        /// Updates the header and line items of an existing purchase order.
        /// </summary>
        Task UpdatePurchaseOrderAsync(PurchaseOrder orderHeader, IEnumerable<PurchaseOrderItem> updatedItems, int updatedByUserId);

        /// <summary>
        /// Cancels a purchase order.
        /// </summary>
        Task CancelPurchaseOrderAsync(int orderId, string reason, int cancelledByUserId);

        /// <summary>
        /// Records the partial or full receipt of a single line item on a purchase order.
        /// </summary>
        Task ReceivePurchaseOrderItemAsync(int orderId, int productIdOfItemToReceive, int quantityReceived, int receivedByUserId);

        /// <summary>
        /// Receives all remaining items for an entire purchase order.
        /// </summary>
        Task ReceiveFullPurchaseOrderAsync(int orderId, int receivedByUserId);
    }
}