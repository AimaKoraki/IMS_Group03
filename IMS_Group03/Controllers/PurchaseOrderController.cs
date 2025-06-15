// --- Controllers/PurchaseOrderController.cs ---
using IMS_Group03.Models;
using IMS_Group03.Services;
using IMS_Group03.ViewModels;
using Microsoft.Extensions.DependencyInjection; 
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
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PurchaseOrderController> _logger;
        private int? _currentUserId;

        #region Properties (Your existing properties are perfect and unchanged)
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

       
        public PurchaseOrderController(IServiceScopeFactory scopeFactory, ILogger<PurchaseOrderController> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            this.PropertyChanged += OnSelectedOrderForFormChanged;
        }

        // This event handler manipulates in-memory data and is correct. No changes needed.
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

        #region Loading and Preparation Methods (Now using Scopes)

        public async Task LoadInitialDataAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty; OnAllPropertiesChanged();

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                    // --- Execute the queries sequentially, not in parallel. ---

                    // 1. Await the first task. The method will pause here until it's done.
                    var orders = await orderService.GetAllOrdersAsync();

                    // 2. Only after the first is complete, start and await the second.
                    var suppliers = await supplierService.GetAllSuppliersAsync();

                    // 3. Only after the second is complete, start and await the third.
                    var products = await productService.GetAllProductsAsync();

                    // Now that all data is safely loaded, update the UI collections.
                    PurchaseOrders.Clear();
                    foreach (var order in orders) PurchaseOrders.Add(order); // OrderBy is in the repo now

                    AvailableSuppliers.Clear();
                    AvailableSuppliers.Add(new Supplier { Id = 0, Name = "-- Select Supplier --" });
                    foreach (var sup in suppliers) AvailableSuppliers.Add(sup);

                    AvailableProducts.Clear();
                    AvailableProducts.Add(new Product { Id = 0, Name = "-- Select Product --" });
                    foreach (var prod in products) AvailableProducts.Add(prod);
                }
                catch (Exception ex)
                {
                    // This will now only catch REAL exceptions, not the DbContext threading error.
                    ErrorMessage = "Failed to load purchase order data.";
                    _logger.LogError(ex, ErrorMessage);
                }
                finally { IsBusy = false; OnAllPropertiesChanged(); }
            }
        }

        // This is a UI-only operation, no DB access, so it's correct.
        public void PrepareNewPurchaseOrder()
        {
            SelectedOrderForForm = new PurchaseOrder { OrderDate = DateTime.Today };
        }

        public async Task PrepareOrderForEditAsync(PurchaseOrder orderToEdit)
        {
            IsBusy = true; OnPropertyChanged(nameof(IsBusy));
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    // Fetch the full order with all details for editing
                    SelectedOrderForForm = await orderService.GetOrderByIdAsync(orderToEdit.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load order {OrderId} for editing.", orderToEdit.Id);
                    ErrorMessage = "Could not load order details.";
                }
                finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
            }
        }
        #endregion

        #region In-Memory Item Management (These methods are correct and need no changes)
        public void AddItemToEditableOrder()
        {
            if (SelectedOrderForForm == null) return;
            var newItem = new PurchaseOrderItem();
            SelectedOrderForForm.PurchaseOrderItems.Add(newItem);
            EditableOrderItems.Add(new PurchaseOrderItemViewModel(newItem, this.AvailableProducts));
        }

        public void RemoveItemFromEditableOrder(PurchaseOrderItemViewModel itemVM)
        {

            if (SelectedOrderForForm != null)
            {
                var modelToRemove = SelectedOrderForForm.PurchaseOrderItems
                                        .FirstOrDefault(i => i.ProductId == itemVM.ProductId && i.PurchaseOrderItemId == itemVM.ToModel().PurchaseOrderItemId);
                if (modelToRemove != null)
                {
                    SelectedOrderForForm.PurchaseOrderItems.Remove(modelToRemove);
                }
            }
            EditableOrderItems.Remove(itemVM);
        }
        public void ClearFormSelection()
        {
            // Setting this to null will hide the form and trigger the
            // OnSelectedOrderForFormChanged event, which clears the item list.
            SelectedOrderForForm = null;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(ErrorMessage));
        }
        public async Task<(bool Success, string Message)> SavePurchaseOrderAsync()
        {
            if (SelectedOrderForForm == null) return (false, "No order to save.");
            if (SelectedOrderForForm.SupplierId == 0) return (false, "A supplier must be selected.");

            // Transfer data from the EditableOrderItems (ViewModels) back to the main model
            SelectedOrderForForm.PurchaseOrderItems = new List<PurchaseOrderItem>(EditableOrderItems.Select(vm => vm.ToModel()));

            IsBusy = true; OnPropertyChanged(nameof(IsBusy));
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    await orderService.CreateOrUpdateOrderAsync(SelectedOrderForForm, _currentUserId);

                    await LoadInitialDataAsync();
                    ClearFormSelection();
                    return (true, "Purchase order saved successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save purchase order.");
                    return (false, "Save failed. A database error occurred.");
                }
                finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
            }
        }

        public async Task<(bool Success, string Message)> CancelPurchaseOrderAsync(int orderId)
        {
            IsBusy = true; OnPropertyChanged(nameof(IsBusy));
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    await orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled, _currentUserId);
                    await LoadInitialDataAsync();
                    return (true, "Order cancelled.");
                }
                catch (Exception ex) { return (false, "Failed to cancel order."); }
                finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
            }
        }

        public async Task<(bool Success, string Message)> UIRecceiveFullOrderAsync(int orderId)
        {
            IsBusy = true; OnPropertyChanged(nameof(IsBusy));
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    await orderService.ReceiveFullOrderAsync(orderId, _currentUserId);
                    await LoadInitialDataAsync();
                    return (true, "Order has been marked as received and stock levels updated.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process receipt for order {OrderId}", orderId);
                    return (false, "Failed to receive order. Check logs for details.");
                }
                finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
            }
        }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(IsBusy));
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }
}