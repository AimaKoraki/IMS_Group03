// --- CORRECTED AND FINALIZED: Services/SupplierService.cs ---
using IMS_Group03.DataAccess.Repositories;
using IMS_Group03.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS_Group03.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(IUnitOfWork unitOfWork, ILogger<SupplierService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Read Operations
        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync() =>
            await _unitOfWork.Suppliers.GetAllAsync();

        public async Task<Supplier?> GetSupplierByIdAsync(int id) =>
            await _unitOfWork.Suppliers.GetByIdAsync(id);

        public async Task<Supplier?> GetSupplierByIdWithProductsAsync(int id) =>
            await _unitOfWork.Suppliers.GetByIdWithProductsAsync(id);

        public async Task<Supplier?> GetSupplierByIdWithOrdersAsync(int id) =>
            await _unitOfWork.Suppliers.GetByIdWithPurchaseOrdersAsync(id);

        public async Task<bool> IsSupplierNameUniqueAsync(string name, int? currentSupplierId = null) =>
            !await _unitOfWork.Suppliers.SupplierNameExistsAsync(name, currentSupplierId);
        #endregion

        #region Write Operations
        public async Task AddSupplierAsync(Supplier supplier)
        {
            if (supplier == null) throw new ArgumentNullException(nameof(supplier));
            if (string.IsNullOrWhiteSpace(supplier.Name)) throw new ArgumentException("Supplier name cannot be empty.", nameof(supplier.Name));

            if (!await IsSupplierNameUniqueAsync(supplier.Name))
            {
                throw new InvalidOperationException($"A supplier with the name '{supplier.Name}' already exists.");
            }

            supplier.DateCreated = DateTime.UtcNow;
            supplier.LastUpdated = DateTime.UtcNow;

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync(); // Commit transaction

            _logger.LogInformation("Created new supplier {SupplierName} with ID {SupplierId}.", supplier.Name, supplier.Id);
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            if (supplier == null) throw new ArgumentNullException(nameof(supplier));

            var existingSupplier = await _unitOfWork.Suppliers.GetByIdAsync(supplier.Id);
            if (existingSupplier == null) throw new KeyNotFoundException($"Supplier with ID {supplier.Id} not found.");
            if (!await IsSupplierNameUniqueAsync(supplier.Name, supplier.Id))
            {
                throw new InvalidOperationException($"Another supplier with the name '{supplier.Name}' already exists.");
            }

            // Map updated properties to the tracked entity
            existingSupplier.Name = supplier.Name;
            existingSupplier.ContactPerson = supplier.ContactPerson;
            existingSupplier.Email = supplier.Email;
            existingSupplier.Phone = supplier.Phone;
            existingSupplier.Address = supplier.Address;
            existingSupplier.LastUpdated = DateTime.UtcNow;

            _unitOfWork.Suppliers.Update(existingSupplier);
            await _unitOfWork.CompleteAsync(); // Commit transaction

            _logger.LogInformation("Updated supplier {SupplierName} (ID: {SupplierId}).", existingSupplier.Name, existingSupplier.Id);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            // The logic here is now transactionally safe because all checks and the final delete
            // are part of the same DbContext instance and will be committed together.
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null) throw new KeyNotFoundException($"Supplier with ID {id} not found.");

            // Check for dependent products
            var products = await _unitOfWork.Products.FindAsync(p => p.SupplierId == id);
            if (products.Any())
            {
                throw new InvalidOperationException($"Cannot delete supplier '{supplier.Name}' as they are assigned to {products.Count()} product(s).");
            }

            // Check for dependent purchase orders
            var orders = await _unitOfWork.PurchaseOrders.FindAsync(po => po.SupplierId == id);
            if (orders.Any())
            {
                throw new InvalidOperationException($"Cannot delete supplier '{supplier.Name}' as they have {orders.Count()} existing purchase order(s).");
            }

            _unitOfWork.Suppliers.Remove(supplier);
            await _unitOfWork.CompleteAsync(); // Commit transaction

            _logger.LogWarning("Deleted supplier {SupplierName} (ID: {SupplierId}).", supplier.Name, supplier.Id);
        }
        #endregion
    }
}