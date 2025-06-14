// --- FINAL GUARANTEED VERSION: IUnitOfWork.cs ---
using System;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ISupplierRepository Suppliers { get; }
        IPurchaseOrderRepository PurchaseOrders { get; }
        IStockMovementRepository StockMovements { get; }
        IUserRepository Users { get; }

        Task<int> CompleteAsync();
    }
}