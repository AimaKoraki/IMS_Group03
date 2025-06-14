// --- FULLY CORRECTED AND FINALIZED: Controllers/PurchaseOrderController.cs ---
using IMS_Group03.Models;
using IMS_Group03.Services;
using IMS_Group03.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IMS_Group03.Controllers
{
    public class PurchaseOrderController : INotifyPropertyChanged
    {
        private readonly IOrderService _orderService;
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;
        private readonly ILogger<PurchaseOrderController> _logger;
        private int? _currentUserId;

        #region Properties
        public ObservableCollection<PurchaseOrder> PurchaseOrders { get; } = new();

        private PurchaseOrder? _selectedOrderForForm;
        public PurchaseOrder? SelectedOrderForForm
        {
            get => _selectedOrderForForm;
            set { _selectedOrderForForm = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PurchaseOrderItemViewModel> EditableOrderItems { get; } = new();
        public ObservableCollection<Supplier> AvailableSuppliers { get; } = new();
        public ObservableCollection<Product> AvailableProducts { get; } = new();
        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public PurchaseOrderController(IOrderService orderService, ISupplierService supplierService, IProductService productService, ILogger<PurchaseOrderController> logger)
        {
            _orderService = orderService;
            _supplierService = supplierService;
            _productService = productService;
            _logger = logger;
            this.PropertyChanged += OnSelectedOrderForFormChanged;
        }

        private void OnSelectedOrderForFormChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedOrderForForm)) return;

            EditableOrderItems.Clear();
            if (SelectedOrderForForm?.PurchaseOrderItems != null)
            {
                foreach (var item in SelectedOrderForForm.PurchaseOrderItems)
                {
                    EditableOrderItems.Add(new PurchaseOrderItemViewModel(item, this.AvailableProducts));
                }
            }
        }

        public void SetCurrentUser(User user)
        {
            _currentUserId = user.Id;
        }

        #region Loading and Preparation Methods
        public async Task LoadInitialDataAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty;
            try
            {
                // FIX: Running sequentially to avoid DbContext threading issues.
                await LoadPurchaseOrdersAsync();
                await LoadSuppliersForFormAsync();
                await LoadProductsForFormAsync();
            }
            catch (Exception ex) { ErrorMessage = "Failed to load initial data."; _logger.LogError(ex, ErrorMessage); }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }

        public async Task LoadPurchaseOrdersAsync()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            PurchaseOrders.Clear();
            foreach (var order in orders.OrderByDescending(o => o.OrderDate)) PurchaseOrders.Add(order);
        }

        private async Task LoadSuppliersForFormAsync()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            AvailableSuppliers.Clear();
            AvailableSuppliers.Add(new Supplier { Id = 0, Name = "-- Select Supplier --" });
            foreach (var sup in suppliers.OrderBy(s => s.Name)) AvailableSuppliers.Add(sup);
        }

        private async Task LoadProductsForFormAsync()
        {
            var products = await _productService.GetAllProductsAsync();
            AvailableProducts.Clear();
            AvailableProducts.Add(new Product { Id = 0, Name = "-- Select Product --" });
            foreach (var prod in products.OrderBy(p => p.Name)) AvailableProducts.Add(prod);
        }

        public void PrepareNewPurchaseOrder()
        {
            SelectedOrderForForm = new PurchaseOrder { OrderDate = DateTime.Today };
        }

        public async Task PrepareOrderForEditAsync(PurchaseOrder orderToEdit)
        {
            IsBusy = true;
            try
            {
                SelectedOrderForForm = await _orderService.GetOrderByIdAsync(orderToEdit.Id);
            }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }
        #endregion

        #region Form Manipulation Methods

        public void AddItemToEditableOrder()
        {
            EditableOrderItems.Add(new PurchaseOrderItemViewModel(new PurchaseOrderItem(), this.AvailableProducts) { IsNew = true });
        }

        public void RemoveItemFromEditableOrder(PurchaseOrderItemViewModel itemVM)
        {
            EditableOrderItems.Remove(itemVM);
        }

        public void ClearFormSelection()
        {
            SelectedOrderForForm = null;
        }
        #endregion

        #region Main Actions (Save, Cancel, Receive)

        public async Task<(bool Success, string Message)> SavePurchaseOrderAsync()
        {
            if (_currentUserId == null) return (false, "Cannot save. No user session found.");
            if (SelectedOrderForForm == null) return (false, "No order data to save.");
            if (SelectedOrderForForm.SupplierId == 0) return (false, "Please select a supplier.");
            if (!EditableOrderItems.Any()) return (false, "Order must have at least one item.");

            IsBusy = true; ErrorMessage = string.Empty;
            try
            {
                var itemsToSave = EditableOrderItems.Select(vm => vm.ToModel()).ToList();
                if (SelectedOrderForForm.Id == 0)
                {
                    await _orderService.CreatePurchaseOrderAsync(SelectedOrderForForm, itemsToSave, _currentUserId.Value);
                }
                else
                {
                    await _orderService.UpdatePurchaseOrderAsync(SelectedOrderForForm, itemsToSave, _currentUserId.Value);
                }
                await LoadPurchaseOrdersAsync();
                ClearFormSelection();
                return (true, "Save successful.");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Save failed: {ex.Message}";
                _logger.LogError(ex, ErrorMessage);
                return (false, ErrorMessage);
            }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }

        public async Task<(bool Success, string Message)> CancelPurchaseOrderAsync(int orderId)
        {
            if (_currentUserId == null) return (false, "Cannot cancel. No user session found.");
            IsBusy = true;
            try
            {
                await _orderService.CancelPurchaseOrderAsync(orderId, "Cancelled by user via UI.", _currentUserId.Value);
                await LoadPurchaseOrdersAsync();
                return (true, "Order cancelled successfully.");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Cancel failed: {ex.Message}";
                _logger.LogError(ex, ErrorMessage);
                return (false, ErrorMessage);
            }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }

        public async Task<(bool Success, string Message)> UIRecceiveFullOrderAsync(int orderId)
        {
            if (_currentUserId == null) return (false, "Cannot receive. No user session found.");
            IsBusy = true;
            try
            {
                await _orderService.ReceiveFullPurchaseOrderAsync(orderId, _currentUserId.Value);
                await LoadPurchaseOrdersAsync();
                return (true, "Order fully received.");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Receive failed: {ex.Message}";
                _logger.LogError(ex, ErrorMessage);
                return (false, ErrorMessage);
            }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}