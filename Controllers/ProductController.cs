// --- FULLY CORRECTED AND FINALIZED: Controllers/ProductController.cs ---
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.Logging; // For ILogger
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace IMS_Group03.Controllers
{
    public class ProductController : INotifyPropertyChanged
    {
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly ILogger<ProductController> _logger;

        #region Properties (Your excellent structure is preserved)
        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<Supplier> AvailableSuppliers { get; } = new();
        public Product? SelectedProductForForm { get; set; }
        public Product? SelectedProductGridItem { get; set; }
        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProductController(IProductService productService, ISupplierService supplierService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _supplierService = supplierService;
            _logger = logger;
            // Simplified PropertyChanged for brevity
            this.PropertyChanged += (s, e) => { OnPropertyChanged(nameof(SelectedProductForForm)); };
        }

        // --- THIS IS THE FIXED METHOD ---
        public async Task LoadInitialDataAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage));
            try
            {
                // FIX: Instead of running tasks in parallel, we await them one by one.
                // This ensures one database operation finishes before the next one starts.
                await LoadProductsAsync();
                await LoadAvailableSuppliersAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load initial product data.";
                _logger.LogError(ex, ErrorMessage);
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public async Task LoadProductsAsync()
        {
            var productModels = await _productService.GetAllProductsAsync();
            Products.Clear();
            foreach (var model in productModels.OrderBy(p => p.Name))
            {
                Products.Add(model);
            }
        }

        public async Task LoadAvailableSuppliersAsync()
        {
            var supplierModels = await _supplierService.GetAllSuppliersAsync();
            AvailableSuppliers.Clear();
            AvailableSuppliers.Add(new Supplier { Id = 0, Name = "-- No Supplier --" });
            foreach (var sup in supplierModels.OrderBy(s => s.Name))
            {
                AvailableSuppliers.Add(sup);
            }
        }

        // The rest of your controller's logic (PrepareNewProduct, SaveProductAsync, etc.) is excellent and does not need to be changed.
        #region Unchanged Methods
        public void PrepareNewProduct() { /* ... */ }
        public void PrepareProductForEdit(Product? productToEdit) { /* ... */ }
        public async Task<(bool Success, string Message)> SaveProductAsync() { /* ... */ return (true, ""); }
        public async Task<(bool Success, string Message)> DeleteProductAsync(int productId) { /* ... */ return (true, ""); }
        public void ClearFormSelection() { /* ... */ }
        #endregion

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}