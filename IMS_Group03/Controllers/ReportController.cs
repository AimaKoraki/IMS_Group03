// --- FULLY CORRECTED AND FINALIZED: Controllers/ReportController.cs ---
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace IMS_Group03.Controllers
{
    public class ReportType { public string Name { get; set; } = string.Empty; public string Key { get; set; } = string.Empty; }

    public class ReportController : INotifyPropertyChanged
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ILogger<ReportController> _logger;
        private readonly int _lowStockThreshold;

        #region Properties
        public ObservableCollection<ReportType> AvailableReportTypes { get; }

        private ReportType _selectedReportType = null!;
        public ReportType SelectedReportType
        {
            get => _selectedReportType;
            set
            {
                if (_selectedReportType != value)
                {
                    _selectedReportType = value;
                    OnPropertyChanged();
                    UpdateParameterVisibility();
                }
            }
        }

        public DateTime? StartDate { get; set; } = DateTime.Today.AddMonths(-1);
        public DateTime? EndDate { get; set; } = DateTime.Today;
        public ObservableCollection<Product> FilterableProducts { get; } = new();
        public Product? SelectedProductFilter { get; set; }
        public Visibility DateRangeParameterVisibility { get; private set; }
        public Visibility ProductParameterVisibility { get; private set; }
        public object? ReportData { get; private set; }
        public DataTable? ReportDataTable { get; private set; }
        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public ReportController(IProductService productService, IOrderService orderService, ILogger<ReportController> logger, IOptions<AppSettings> appSettings)
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

            SelectedReportType = AvailableReportTypes.First();
        }

        public async Task LoadInitialDataAsync()
        {
            IsBusy = true; OnPropertyChanged(nameof(IsBusy));
            try
            {
                var products = await _productService.GetAllProductsAsync();
                FilterableProducts.Clear();
                FilterableProducts.Add(new Product { Id = 0, Name = "-- All Products --" });
                foreach (var p in products.OrderBy(pr => pr.Name))
                {
                    FilterableProducts.Add(p);
                }
            }
            catch (Exception ex) { ErrorMessage = "Error loading filter data."; _logger.LogError(ex, ErrorMessage); }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }

        private void UpdateParameterVisibility()
        {
            DateRangeParameterVisibility = Visibility.Collapsed;
            ProductParameterVisibility = Visibility.Collapsed;

            if (SelectedReportType?.Key == "PO_DATE_RANGE")
            {
                DateRangeParameterVisibility = Visibility.Visible;
            }

            OnPropertyChanged(nameof(DateRangeParameterVisibility));
            OnPropertyChanged(nameof(ProductParameterVisibility));
        }

        public async Task GenerateReportAsync()
        {
            if (SelectedReportType == null || SelectedReportType.Key == "NONE")
            {
                ErrorMessage = "Please select a report type.";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            IsBusy = true; ErrorMessage = string.Empty;
            ReportData = null; ReportDataTable = null;
            OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(ReportData)); OnPropertyChanged(nameof(ReportDataTable));

            try
            {
                switch (SelectedReportType.Key)
                {
                    case "LOW_STOCK":
                        var lowStockProducts = await _productService.GetLowStockProductsAsync(_lowStockThreshold);
                        ReportData = new ObservableCollection<Product>(lowStockProducts);
                        break;

                    case "PO_DATE_RANGE":
                        if (!StartDate.HasValue || !EndDate.HasValue) throw new ArgumentException("Start and End dates are required.");

                        var orders = await _orderService.GetAllOrdersAsync();
                        var filteredOrders = orders.Where(o => o.OrderDate.Date >= StartDate.Value.Date && o.OrderDate.Date <= EndDate.Value.Date);

                        var poDt = new DataTable("PurchaseOrders");
                        poDt.Columns.Add("ID", typeof(int));
                        poDt.Columns.Add("OrderDate", typeof(DateTime));
                        poDt.Columns.Add("Supplier", typeof(string));
                        poDt.Columns.Add("Status", typeof(string));
                        poDt.Columns.Add("ItemCount", typeof(int));
                        poDt.Columns.Add("TotalAmount", typeof(decimal));

                        foreach (var order in filteredOrders)
                        {
                            poDt.Rows.Add(order.Id, order.OrderDate, order.Supplier.Name, order.Status.ToString(), order.PurchaseOrderItems.Count, order.TotalAmount);
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
                OnPropertyChanged(nameof(ReportData));
                OnPropertyChanged(nameof(ReportDataTable));
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}