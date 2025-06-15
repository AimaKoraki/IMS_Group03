// --- Services/IOrderService.cs ---
using IMS_Group03.Models;
using System; // Required for DateTime
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS_Group03.Services
{
    /// <summary>
    /// Defines the contract for business logic related to Purchase Orders.
    /// These methods orchestrate one or more repository calls within a single transaction
    /// via the UnitOfWork pattern.
    /// </summary>
    public interface IOrderService
    {
        #region Read Operations

        /// <summary>
        /// Gets a single purchase order by its ID, ensuring all related items and supplier data are included.
        /// </summary>
        Task<PurchaseOrder?> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// Gets all purchase orders, including their related data.
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetAllOrdersAsync();

        /// <summary>
        /// Gets all purchase orders that match a specific status.
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetOrdersByStatusAsync(OrderStatus status);

        /// <summary>
        /// Gets all purchase orders within a given date range for reporting.
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets all purchase orders for a specific supplier.
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetOrdersForSupplierAsync(int supplierId);

        #endregion

        #region Write Operations (Transactional)

        /// <summary>
        /// Creates a new purchase order or updates an existing one in a single transaction.
        /// This method handles both add and edit scenarios.
        /// </summary>
        /// <param name="order">The purchase order model, including its items.</param>
        /// <param name="userId">The ID of the user performing the action for auditing.</param>
        Task CreateOrUpdateOrderAsync(PurchaseOrder order, int? userId);

        /// <summary>
        /// Updates the status of an existing order (e.g., to 'Cancelled' or 'On Hold').
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="newStatus">The new status for the order.</param>
        /// <param name="userId">The ID of the user performing the action.</param>
        Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, int? userId);

        /// <summary>
        /// Marks an entire order as received and updates stock levels for all items in a single transaction.
        /// </summary>
        /// <param name="orderId">The ID of the order being received.</param>
        /// <param name="userId">The ID of the user performing the action.</param>
        Task ReceiveFullOrderAsync(int orderId, int? userId);

        /// <summary>
        /// Records the partial or full receipt of a SINGLE line item on a purchase order.
        /// Useful for future implementation of partial receipts.
        /// </summary>
        Task ReceivePurchaseOrderItemAsync(int orderId, int purchaseOrderItemId, int quantityReceived, int? receivedByUserId);

        #endregion
    }
}