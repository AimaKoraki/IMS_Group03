// File: DataAccess/Repositories/ISupplierRepository.cs
using IMS_Group03.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        /// <summary>
        /// Gets all suppliers including their associated products.
        /// </summary>
        Task<IEnumerable<Supplier>> GetAllWithProductsAsync();

        /// <summary>
        /// Gets a specific supplier by ID, including their associated products.
        /// </summary>
        Task<Supplier> GetByIdWithProductsAsync(int supplierId);

        /// <summary>
        /// Gets all suppliers including their associated purchase orders.
        /// </summary>
        Task<IEnumerable<Supplier>> GetAllWithPurchaseOrdersAsync();

        /// <summary>
        /// Gets a specific supplier by ID, including their associated purchase orders.
        /// </summary>
        Task<Supplier> GetByIdWithPurchaseOrdersAsync(int supplierId);

        /// <summary>
        /// Checks if a supplier with the given name already exists, optionally excluding a specific supplier ID.
        /// </summary>
        Task<bool> SupplierNameExistsAsync(string name, int? currentSupplierId = null);
    }
}