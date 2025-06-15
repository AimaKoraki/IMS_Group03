// --- DataAccess/Repositories/IPurchaseOrderRepository.cs ---
using IMS_Group03.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        /// <summary>
        /// Gets a purchase order by ID, including its supplier, order items, and the product for each item.
        /// </summary>
        // --- FIX: The return type is now nullable to reflect that an order might not be found. ---
        Task<PurchaseOrder?> GetByIdWithDetailsAsync(int purchaseOrderId);

        /// <summary>
        /// Gets all purchase orders, including their suppliers and order items with products.
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetAllWithDetailsAsync();

        /// <summary>
        /// Gets purchase orders within a specific date range, with details.
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetOrdersByDateRangeWithDetailsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets purchase orders with a specific status, with details.
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetOrdersByStatusWithDetailsAsync(OrderStatus status);

        /// <summary>
        /// Gets purchase orders for a specific supplier, with details.
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetOrdersBySupplierWithDetailsAsync(int supplierId);

        /// <summary>
        /// Gets purchase orders that contain a specific product.
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetOrdersContainingProductAsync(int productId);
    }
}