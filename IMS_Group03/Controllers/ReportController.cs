// --- Controllers/ReportController.cs ---
using IMS_Group03.Config;
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace IMS_Group03.Controllers
{
    public class ReportType { public string Name { get; set; } = string.Empty; public string Key { get; set; } = string.Empty; }

    public class ReportController : INotifyPropertyChanged
    {
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReportController> _logger;
        private readonly int _lowStockThreshold;

        #region Properties (Your existing properties are perfect and unchanged)
        public ObservableCollection<ReportType> AvailableReportTypes { get; }
        public ReportType SelectedReportType { get; set; } 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ObservableCollection<Product> FilterableProducts { get; } = new();
        public Product? SelectedProductFilter { get; set; }
        public Visibility DateRangeParameterVisibility { get; private set; }
        public Visibility ProductParameterVisibility { get; private set; }
        public object? ReportData { get; private set; }
        public DataTable? ReportDataTable { get; private set; }
        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

       
        public ReportController(
            IServiceScopeFactory scopeFactory,
            ILogger<ReportController> logger,
            IOptions<AppSettings> appSettings)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _lowStockThreshold = appSettings.Value.DefaultLowStockThreshold;

            AvailableReportTypes = new ObservableCollection<ReportType>
            {
                new ReportType { Name = "-- Select a Report --", Key = "NONE" },
                new ReportType { Name = "Low Stock Report", Key = "LOW_STOCK" },
                new ReportType { Name = "Purchase Orders by Date", Key = "PO_DATE_RANGE" }
            };
            SelectedReportType = AvailableReportTypes.First();
        }

        // ---  Load initial dropdown data inside a scope. ---
        public async Task LoadInitialDataAsync()
        {
            IsBusy = true; OnPropertyChanged(nameof(IsBusy));
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    var products = await productService.GetAllProductsAsync();
                    FilterableProducts.Clear();
                    FilterableProducts.Add(new Product { Id = 0, Name = "-- All Products --" });
                    foreach (var p in products.OrderBy(p => p.Name)) FilterableProducts.Add(p);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load filterable products for reports.");
                    ErrorMessage = "Could not load filter data.";
                    OnPropertyChanged(nameof(ErrorMessage));
                }
                finally
                {
                    IsBusy = false; OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

      
        public void UpdateParameterVisibility(Visibility v)
        {
            DateRangeParameterVisibility = SelectedReportType.Key == "PO_DATE_RANGE" ? Visibility.Visible : Visibility.Collapsed;
            ProductParameterVisibility = v; // Example, adapt as needed
            OnPropertyChanged(nameof(DateRangeParameterVisibility));
            OnPropertyChanged(nameof(ProductParameterVisibility));
        }

       
        public async Task GenerateReportAsync()
        {
            if (SelectedReportType?.Key == "NONE")
            {
                ErrorMessage = "Please select a report to generate.";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            ReportData = null;
            ReportDataTable = null;
            OnAllPropertiesChanged();

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    // Resolve services needed for this report run from the scope.
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    switch (SelectedReportType.Key)
                    {
                        case "LOW_STOCK":
                            var lowStockProducts = await productService.GetLowStockProductsAsync(_lowStockThreshold);
                            ReportData = new ObservableCollection<Product>(lowStockProducts);
                            break;

                        case "PO_DATE_RANGE":
                            if (!StartDate.HasValue || !EndDate.HasValue) throw new ArgumentException("Start and End dates are required.");

                            // Let the service layer do the heavy lifting.
                            var filteredOrders = await orderService.GetOrdersByDateRangeAsync(StartDate.Value, EndDate.Value);

                            var poDt = new DataTable("PurchaseOrders");
                            poDt.Columns.Add("ID", typeof(int));
                            poDt.Columns.Add("OrderDate", typeof(DateTime));
                            poDt.Columns.Add("Supplier", typeof(string));
                            poDt.Columns.Add("Status", typeof(string));
                            poDt.Columns.Add("ItemCount", typeof(int));

                            foreach (var order in filteredOrders)
                            {
                                // Ensure Supplier is loaded! Service method should use .Include(o => o.Supplier)
                                poDt.Rows.Add(order.Id, order.OrderDate, order.Supplier?.Name ?? "N/A", order.Status.ToString(), order.PurchaseOrderItems.Count);
                            }
                            ReportDataTable = poDt;
                            break;

                        default:
                            ErrorMessage = "Selected report type is not implemented.";
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate report '{ReportName}'.", SelectedReportType.Name);
                    ErrorMessage = $"Failed to generate report. A database error occurred.";
                }
                finally
                {
                    IsBusy = false;
                    OnAllPropertiesChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(ReportData));
            OnPropertyChanged(nameof(ReportDataTable));
            OnPropertyChanged(nameof(IsBusy));
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }
}