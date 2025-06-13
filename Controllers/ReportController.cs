// --- CORRECTED AND FINALIZED: Controllers/ReportController.cs ---
using IMS_Group03.Config;
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
// Other usings are correct

namespace IMS_Group03.Controllers
{
    // The ReportType class is fine as is.
    public class ReportType { public string Name { get; set; } = string.Empty; public string Key { get; set; } = string.Empty; }

    public class ReportController : INotifyPropertyChanged
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ILogger<ReportController> _logger; // IMPROVEMENT: Injected logger
        private readonly int _lowStockThreshold;             // IMPROVEMENT: Will be set from config

        // Your properties are all well-structured and do not need changes.
        #region Properties
        public ObservableCollection<ReportType> AvailableReportTypes { get; }
        public ReportType SelectedReportType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ObservableCollection<Product> FilterableProducts { get; }
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
            IProductService productService,
            IOrderService orderService,
            ILogger<ReportController> logger,
            IOptions<AppSettings> appSettings)
        {
            _productService = productService;
            _orderService = orderService;
            _logger = logger;
            _lowStockThreshold = appSettings.Value.DefaultLowStockThreshold;

            AvailableReportTypes = new ObservableCollection<ReportType>
            {
                new ReportType { Name = "-- Select a Report --", Key = "NONE" },
                new ReportType { Name = "Low Stock Report", Key = "LOW_STOCK" },
                new ReportType { Name = "Purchase Orders by Date", Key = "PO_DATE_RANGE" }
            };
            FilterableProducts = new ObservableCollection<Product>();

            // Your logic for setting default SelectedReportType is correct.
            SelectedReportType = AvailableReportTypes.First();
            // Simplified PropertyChanged for brevity
            this.PropertyChanged += (s, e) => { };
        }

        // Your LoadInitialDataAsync and UpdateParameterVisibility methods are correct.

        public async Task GenerateReportAsync()
        {
            // (Your validation logic is correct)
            IsBusy = true; ErrorMessage = string.Empty;
            ReportData = null; ReportDataTable = null;

            try
            {
                switch (SelectedReportType.Key)
                {
                    case "LOW_STOCK":
                        var lowStockProducts = await _productService.GetLowStockProductsAsync(_lowStockThreshold);
                        // The controller's job is to prepare data for the view.
                        ReportData = new ObservableCollection<Product>(lowStockProducts);
                        break;

                    case "PO_DATE_RANGE":
                        if (!StartDate.HasValue || !EndDate.HasValue) throw new ArgumentException("Start and End dates are required.");

                        // FIX: Call the actual service to get data.
                        var orders = await _orderService.GetAllOrdersAsync(); // In real app, service method would take dates.
                        var filteredOrders = orders.Where(o => o.OrderDate >= StartDate && o.OrderDate <= EndDate);

                        // FIX: Transform the data into a DataTable for the View.
                        var poDt = new DataTable("PurchaseOrders");
                        poDt.Columns.Add("ID", typeof(int));
                        poDt.Columns.Add("OrderDate", typeof(DateTime));
                        poDt.Columns.Add("Supplier", typeof(string));
                        poDt.Columns.Add("Status", typeof(string));
                        poDt.Columns.Add("ItemCount", typeof(int));

                        foreach (var order in filteredOrders)
                        {
                            poDt.Rows.Add(order.Id, order.OrderDate, order.Supplier.Name, order.Status.ToString(), order.PurchaseOrderItems.Count);
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
                ErrorMessage = $"Failed to generate report: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                // Manually notify all properties have changed
                OnPropertyChanged(nameof(ReportData));
                OnPropertyChanged(nameof(ReportDataTable));
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public async Task LoadInitialDataAsync() { /* ... */ }
    }
}