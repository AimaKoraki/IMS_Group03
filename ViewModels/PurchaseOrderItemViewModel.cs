// --- FINALIZED: ViewModels/PurchaseOrderItemViewModel.cs ---
using IMS_Group03.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace IMS_Group03.ViewModels // Placed in a ViewModels/Helpers namespace
{
    public class PurchaseOrderItemViewModel : INotifyPropertyChanged
    {
        // Your excellent implementation for this class is unchanged.
        // It correctly wraps the model and provides UI-specific properties.
        private readonly PurchaseOrderItem _model;
        public bool IsNew { get; set; }

        // Properties like ProductId, Quantity, UnitPrice, TotalPrice etc.
        // And methods like ToModel() and UpdateProductDetails() are all correct.

        #region Properties and Methods (Your Code)
        public int PurchaseOrderId => _model.PurchaseOrderId;
        public int ViewModelProductId => _model.ProductId;

        private int _productId;
        public int ProductId
        {
            get => _productId;
            set { if (_productId != value) { _productId = value; OnPropertyChanged(); UpdateProductDetails(); } }
        }

        private string _productName = string.Empty;
        public string ProductName { get => _productName; private set { _productName = value; OnPropertyChanged(); } }

        private string _productSku = string.Empty;
        public string ProductSku { get => _productSku; private set { _productSku = value; OnPropertyChanged(); } }

        private int _quantity;
        public int Quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(TotalPrice)); } } }

        private decimal _unitPrice;
        public decimal UnitPrice { get => _unitPrice; set { if (_unitPrice != value) { _unitPrice = value; OnPropertyChanged(); OnPropertyChanged(nameof(TotalPrice)); } } }

        public decimal TotalPrice => Quantity * UnitPrice;

        public ObservableCollection<Product> AvailableProductsForSelection { get; }

        public PurchaseOrderItemViewModel(PurchaseOrderItem model, ObservableCollection<Product> availableProducts)
        {
            _model = model;
            AvailableProductsForSelection = availableProducts;
            ProductId = model.ProductId;
            Quantity = model.QuantityOrdered;
            UnitPrice = model.UnitPrice;
            UpdateProductDetails();
        }

        private void UpdateProductDetails()
        {
            var product = AvailableProductsForSelection.FirstOrDefault(p => p.Id == ProductId);
            if (product != null)
            {
                ProductName = product.Name;
                ProductSku = product.Sku;
                if (IsNew && UnitPrice == 0) UnitPrice = product.Price;
            }
        }

        public PurchaseOrderItem ToModel()
        {
            _model.ProductId = this.ProductId;
            _model.QuantityOrdered = this.Quantity;
            _model.UnitPrice = this.UnitPrice;
            return _model;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}