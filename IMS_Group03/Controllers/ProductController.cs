// ---  Controllers/ProductController.cs ---
using IMS_Group03.Models;
using IMS_Group03.Services;
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
    public class ProductController : INotifyPropertyChanged
    {
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProductController> _logger;
        private int? _currentUserId; // For auditing, if needed.

        #region Properties (Your existing properties are perfect and unchanged)
        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<Supplier> AvailableSuppliers { get; } = new();
        public Product? SelectedProductForForm { get; private set; }
        public Product? SelectedProductGridItem { get; set; }
        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        // ---The constructor is updated to inject IServiceScopeFactory. ---
        public ProductController(IServiceScopeFactory scopeFactory, ILogger<ProductController> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        
        public void SetCurrentUser(User user)
        {
            _currentUserId = user.Id;
        }

        #region Loading and Preparation Methods (Now using Scopes)

        // --- All data loading happens inside a dedicated scope. ---
        public async Task LoadInitialDataAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty; OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage));

            // Create one scope for this entire compound operation.
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    // Resolve services from the scope.
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();

                    // Load products
                    var productModels = await productService.GetAllProductsAsync();
                    Products.Clear();
                    foreach (var model in productModels.OrderBy(p => p.Name)) Products.Add(model);

                    // Load suppliers
                    var supplierModels = await supplierService.GetAllSuppliersAsync();
                    AvailableSuppliers.Clear();
                    AvailableSuppliers.Add(new Supplier { Id = 0, Name = "-- No Supplier --" }); // Placeholder
                    foreach (var sup in supplierModels.OrderBy(s => s.Name)) AvailableSuppliers.Add(sup);
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Failed to load initial product data.";
                    _logger.LogError(ex, ErrorMessage);
                    OnPropertyChanged(nameof(ErrorMessage));
                }
                finally
                {
                    IsBusy = false; OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

        
        public void PrepareNewProduct()
        {
            ErrorMessage = string.Empty;
            SelectedProductGridItem = null;
            SelectedProductForForm = new Product { LowStockThreshold = 10 };
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(SelectedProductGridItem));
            OnPropertyChanged(nameof(SelectedProductForForm));
        }

        public void PrepareProductForEdit(Product? productToEdit)
        {
            ErrorMessage = string.Empty;
            if (productToEdit == null) { SelectedProductForForm = null; }
            else
            {
                SelectedProductForForm = new Product // Correctly creates a copy for editing
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
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(SelectedProductForForm));
        }

        public void ClearFormSelection()
        {
            SelectedProductForForm = null;
            OnPropertyChanged(nameof(SelectedProductForForm));
        }
        #endregion

        #region Save/Delete Methods (Now using Scopes)

        
        public async Task<(bool Success, string Message)> SaveProductAsync()
        {
            if (SelectedProductForForm == null) return (false, "No product data to save.");
            if (string.IsNullOrWhiteSpace(SelectedProductForForm.Name) || string.IsNullOrWhiteSpace(SelectedProductForForm.Sku))
                return (false, "Product Name and SKU are required.");

            IsBusy = true; OnPropertyChanged(nameof(IsBusy));

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                    if (SelectedProductForForm.SupplierId == 0) SelectedProductForForm.SupplierId = null;

                    if (SelectedProductForForm.Id == 0) // New product
                    {
                        await productService.AddProductAsync(SelectedProductForForm);
                    }
                    else // Existing product
                    {
                        await productService.UpdateProductAsync(SelectedProductForForm);
                    }

                    // After saving, reload the main product list to show the changes.
                    // This can be done in a new scope or reuse the existing one.

                    await LoadInitialDataAsync(); // This will create its own new scope.

                    ClearFormSelection();
                    return (true, "Save successful.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save product.");
                    return (false, $"Save failed: A database error occurred.");
                }
                finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
            }
        }

        // --- The delete operation is wrapped in its own scope. ---
        public async Task<(bool Success, string Message)> DeleteProductAsync(int productId)
        {
            IsBusy = true; OnPropertyChanged(nameof(IsBusy));

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    await productService.DeleteProductAsync(productId);

                    // Refresh data after deletion
                    await LoadInitialDataAsync();

                    ClearFormSelection();
                    return (true, "Product deleted successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete product {ProductId}", productId);
                    return (false, $"Delete failed: The product might be in use.");
                }
                finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
            }
        }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}