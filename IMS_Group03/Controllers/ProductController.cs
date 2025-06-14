// --- FINAL, GUARANTEED CORRECT VERSION: Controllers/ProductController.cs ---
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IMS_Group03.Controllers
{
    public class ProductController : INotifyPropertyChanged
    {
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly ILogger<ProductController> _logger;

        #region Properties
        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<Supplier> AvailableSuppliers { get; } = new();

        // --- THE FINAL FIX IS HERE ---
        // Changed the setter to 'private set' to match the working SupplierController.
        public Product? SelectedProductForForm { get; private set; }

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
        }

        #region Loading and Preparation Methods
        public async Task LoadInitialDataAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage));
            try
            {
                await LoadProductsAsync();
                await LoadAvailableSuppliersAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load initial product data.";
                _logger.LogError(ex, ErrorMessage);
            }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }

        public async Task LoadProductsAsync()
        {
            var productModels = await _productService.GetAllProductsAsync();
            Products.Clear();
            foreach (var model in productModels.OrderBy(p => p.Name)) Products.Add(model);
        }

        private async Task LoadAvailableSuppliersAsync()
        {
            var supplierModels = await _supplierService.GetAllSuppliersAsync();
            AvailableSuppliers.Clear();
            AvailableSuppliers.Add(new Supplier { Id = 0, Name = "-- No Supplier --" });
            foreach (var sup in supplierModels.OrderBy(s => s.Name)) AvailableSuppliers.Add(sup);
        }

        public void PrepareNewProduct()
        {
            SelectedProductGridItem = null;
            SelectedProductForForm = new Product { LowStockThreshold = 10 };
            OnPropertyChanged(nameof(SelectedProductForForm));
        }

        public void PrepareProductForEdit(Product? productToEdit)
        {
            if (productToEdit == null)
            {
                SelectedProductForForm = null;
            }
            else
            {
                SelectedProductForForm = new Product
                {
                    Id = productToEdit.Id,
                    Name = productToEdit.Name,
                    Sku = productToEdit.Sku,
                    Description = productToEdit.Description,
                    QuantityInStock = productToEdit.QuantityInStock,
                    Price = productToEdit.Price,
                    LowStockThreshold = productToEdit.LowStockThreshold,
                    SupplierId = productToEdit.SupplierId
                };
            }
            OnPropertyChanged(nameof(SelectedProductForForm));
        }

        public void ClearFormSelection()
        {
            SelectedProductForForm = null;
            OnPropertyChanged(nameof(SelectedProductForForm));
        }
        #endregion

        #region Save/Delete Methods
        public async Task<(bool Success, string Message)> SaveProductAsync()
        {
            if (SelectedProductForForm == null) return (false, "No product data to save.");
            if (string.IsNullOrWhiteSpace(SelectedProductForForm.Name) || string.IsNullOrWhiteSpace(SelectedProductForForm.Sku))
                return (false, "Product Name and SKU are required.");

            IsBusy = true; ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage));
            try
            {
                if (SelectedProductForForm.SupplierId == 0) SelectedProductForForm.SupplierId = null;

                if (SelectedProductForForm.Id == 0)
                {
                    await _productService.AddProductAsync(SelectedProductForForm);
                }
                else
                {
                    await _productService.UpdateProductAsync(SelectedProductForForm);
                }

                await LoadProductsAsync();
                ClearFormSelection();
                return (true, "Save successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save product.");
                ErrorMessage = $"Save failed: {ex.Message}";
                OnPropertyChanged(nameof(ErrorMessage));
                return (false, ErrorMessage);
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public async Task<(bool Success, string Message)> DeleteProductAsync(int productId)
        {
            IsBusy = true; ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage));
            try
            {
                await _productService.DeleteProductAsync(productId);
                await LoadProductsAsync();
                ClearFormSelection();
                return (true, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product {ProductId}", productId);
                ErrorMessage = $"Delete failed: {ex.Message}";
                OnPropertyChanged(nameof(ErrorMessage));
                return (false, ErrorMessage);
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}