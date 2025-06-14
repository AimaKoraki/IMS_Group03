// --- VERIFIED AND CORRECT: Services/ISupplierService.cs ---
using IMS_Group03.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMS_Group03.DataAccess.Repositories;
namespace IMS_Group03.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier?> GetSupplierByIdAsync(int id);
        Task<Supplier?> GetSupplierByIdWithProductsAsync(int id);
        Task<Supplier?> GetSupplierByIdWithOrdersAsync(int id);
        Task AddSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(int id);
        Task<bool> IsSupplierNameUniqueAsync(string name, int? currentSupplierId = null);
    }
}