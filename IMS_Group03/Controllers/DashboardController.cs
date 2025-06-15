// ---  Controllers/DashboardController.cs ---
using IMS_Group03.Config;
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IMS_Group03.Controllers
{
    public class DashboardController : INotifyPropertyChanged
    {


        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DashboardController> _logger;
        private readonly int _lowStockThreshold;

        #region Properties (Your existing properties are perfect and unchanged)
        public int TotalProducts { get; private set; }
        public int TotalSuppliers { get; private set; }
        public int LowStockItemsCount { get; private set; }
        public int PendingPurchaseOrders { get; private set; }
        public ObservableCollection<Product> LowStockPreview { get; } = new();
        public ObservableCollection<PurchaseOrder> RecentPendingOrdersPreview { get; } = new();
        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion


        public DashboardController(
            IServiceScopeFactory scopeFactory, 
            ILogger<DashboardController> logger,
            IOptions<AppSettings> appSettings)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _lowStockThreshold = appSettings.Value.DefaultLowStockThreshold;
        }

        
        public async Task LoadDashboardDataAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty;
            OnAllPropertiesChanged();

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    

                    // 1. Await the first task.
                    var products = await productService.GetAllProductsAsync();

                    // 2. Await the second task.
                    var suppliers = await supplierService.GetAllSuppliersAsync();

                    // 3. Await the third task.
                    var lowStockItems = await productService.GetLowStockProductsAsync(_lowStockThreshold);

                    // 4. Await the fourth task.
                    var pendingOrders = await orderService.GetOrdersByStatusAsync(OrderStatus.Pending);

                    // Now that all data is safely loaded, update the UI properties.
                    TotalProducts = products.Count();
                    TotalSuppliers = suppliers.Count();
                    LowStockItemsCount = lowStockItems.Count();
                    PendingPurchaseOrders = pendingOrders.Count();

                    LowStockPreview.Clear();
                    foreach (var item in lowStockItems.Take(5)) { LowStockPreview.Add(item); }

                    RecentPendingOrdersPreview.Clear();
                    foreach (var item in pendingOrders.OrderByDescending(o => o.OrderDate).Take(5)) { RecentPendingOrdersPreview.Add(item); }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load dashboard data.");
                    ErrorMessage = "A database error occurred while loading dashboard data.";
                }
                finally
                {
                    IsBusy = false;
                    OnAllPropertiesChanged();
                }
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Helper method to simplify notifying the UI of all property changes after a load operation.
        /// </summary>
        private void OnAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(IsBusy));
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(TotalProducts));
            OnPropertyChanged(nameof(TotalSuppliers));
            OnPropertyChanged(nameof(LowStockItemsCount));
            OnPropertyChanged(nameof(PendingPurchaseOrders));
        }
    }
}