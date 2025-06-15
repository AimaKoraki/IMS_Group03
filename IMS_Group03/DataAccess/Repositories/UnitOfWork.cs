// ---  UnitOfWork.cs ---
using System.Threading.Tasks;
using IMS_Group03.DataAccess;
namespace IMS_Group03.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // The UnitOfWork now receives fully-formed repositories from the DI container.
        public IProductRepository Products { get; }
        public ISupplierRepository Suppliers { get; }
        public IPurchaseOrderRepository PurchaseOrders { get; }
        public IStockMovementRepository StockMovements { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(
            AppDbContext context,
            IProductRepository productRepository,
            ISupplierRepository supplierRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IStockMovementRepository stockMovementRepository,
            IUserRepository userRepository)
        {
            _context = context;
            Products = productRepository;
            Suppliers = supplierRepository;
            PurchaseOrders = purchaseOrderRepository;
            StockMovements = stockMovementRepository;
            Users = userRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}