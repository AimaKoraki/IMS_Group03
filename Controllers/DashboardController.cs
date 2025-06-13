// --- CORRECTED AND FINALIZED: Controllers/DashboardController.cs ---
using IMS_Group03.Config; // To reference the AppSettings class
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.Logging;    // For ILogger
using Microsoft.Extensions.Options;  // For IOptions
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
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly IOrderService _orderService;
        private readonly ILogger<DashboardController> _logger; // IMPROVEMENT: Injected logger
        private readonly int _lowStockThreshold;               // IMPROVEMENT: Will be set from config

        #region Properties (Your code is correct here)
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

        // IMPROVEMENT: Constructor now injects ILogger and IOptions<AppSettings>
        public DashboardController(
            IProductService productService,
            ISupplierService supplierService,
            IOrderService orderService,
            ILogger<DashboardController> logger,
            IOptions<AppSettings> appSettings) // Gets the "AppSettings" section from your json
        {
            _productService = productService;
            _supplierService = supplierService;
            _orderService = orderService;
            _logger = logger;

            // Set the threshold from the configuration file.
            _lowStockThreshold = appSettings.Value.DefaultLowStockThreshold;

            // Simplified PropertyChanged for brevity
            this.PropertyChanged += (s, e) => { };
        }

        public async Task LoadDashboardDataAsync()
        {
            IsBusy = true;
            OnPropertyChanged(nameof(IsBusy));
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(ErrorMessage));

            try
            {
                // Your excellent parallel loading logic is preserved.
                var productsTask = _productService.GetAllProductsAsync();
                var suppliersTask = _supplierService.GetAllSuppliersAsync();
                var lowStockTask = _productService.GetLowStockProductsAsync(_lowStockThreshold);
                var pendingOrdersTask = _orderService.GetOrdersByStatusAsync(OrderStatus.Pending);

                await Task.WhenAll(productsTask, suppliersTask, lowStockTask, pendingOrdersTask);

                TotalProducts = (await productsTask).Count();
                TotalSuppliers = (await suppliersTask).Count();
                var lowStockItems = await lowStockTask;
                var pendingOrders = await pendingOrdersTask;

                LowStockItemsCount = lowStockItems.Count();
                LowStockPreview.Clear();
                foreach (var item in lowStockItems.Take(5)) { LowStockPreview.Add(item); }

                PendingPurchaseOrders = pendingOrders.Count();
                RecentPendingOrdersPreview.Clear();
                foreach (var item in pendingOrders.OrderByDescending(o => o.OrderDate).Take(5)) { RecentPendingOrdersPreview.Add(item); }
            }
            catch (Exception ex)
            {
                // IMPROVEMENT: Use the injected logger for structured error logging.
                _logger.LogError(ex, "Failed to load dashboard data.");
                ErrorMessage = "Failed to load dashboard data. Please try again later.";
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                // Manually trigger OnPropertyChanged for all updated properties
                OnPropertyChanged(nameof(TotalProducts));
                OnPropertyChanged(nameof(TotalSuppliers));
                OnPropertyChanged(nameof(LowStockItemsCount));
                OnPropertyChanged(nameof(PendingPurchaseOrders));
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}