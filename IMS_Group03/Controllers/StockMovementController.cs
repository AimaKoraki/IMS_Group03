// --- FULLY CORRECTED AND FINALIZED: Controllers/StockMovementController.cs ---
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
    public class StockMovementController : INotifyPropertyChanged
    {
        private readonly IStockMovementService _stockMovementService;
        private readonly IProductService _productService;
        private readonly ILogger<StockMovementController> _logger;
        private int? _currentUserId;

        #region Properties
        public ObservableCollection<StockMovement> StockMovements { get; } = new();

        private Product? _selectedProductFilter;
        public Product? SelectedProductFilter
        {
            get => _selectedProductFilter;
            set
            {
                if (_selectedProductFilter != value)
                {
                    _selectedProductFilter = value;
                    OnPropertyChanged();
                    // Automatically load movements when filter changes
                    if (_selectedProductFilter?.Id > 0)
                    {
                        _ = LoadMovementsForProductAsync(_selectedProductFilter.Id);
                    }
                    else
                    {
                        StockMovements.Clear();
                    }
                }
            }
        }

        public ObservableCollection<Product> AvailableProducts { get; } = new();
        public int AdjustmentProductId { get; set; }
        public string AdjustmentNewQuantityInput { get; set; } = "0";
        public string AdjustmentReason { get; set; } = string.Empty;
        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public StockMovementController(IStockMovementService stockMovementService, IProductService productService, ILogger<StockMovementController> logger)
        {
            _stockMovementService = stockMovementService;
            _productService = productService;
            _logger = logger;
        }

        public void SetCurrentUser(User user)
        {
            _currentUserId = user.Id;
        }

        #region Loading and Preparation Methods
        public async Task LoadInitialDataAsync()
        {
            IsBusy = true; OnPropertyChanged(nameof(IsBusy));
            try
            {
                var products = await _productService.GetAllProductsAsync();
                AvailableProducts.Clear();
                AvailableProducts.Add(new Product { Id = 0, Name = "-- Select a Product to View History --" });
                foreach (var prod in products.OrderBy(p => p.Name))
                {
                    AvailableProducts.Add(prod);
                }
            }
            catch (Exception ex) { ErrorMessage = "Failed to load product list."; _logger.LogError(ex, ErrorMessage); }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }

        public async Task LoadMovementsForProductAsync(int productId)
        {
            IsBusy = true; ErrorMessage = string.Empty; OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage));
            try
            {
                var movements = await _stockMovementService.GetMovementsForProductAsync(productId);
                StockMovements.Clear();
                foreach (var movement in movements.OrderByDescending(m => m.MovementDate))
                {
                    StockMovements.Add(movement);
                }
            }
            catch (Exception ex) { ErrorMessage = $"Failed to load movements: {ex.Message}"; _logger.LogError(ex, ErrorMessage); }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }
        #endregion

        #region Adjustment Form Logic
        public async Task<(bool Success, string Message)> PerformStockAdjustmentAsync()
        {
            if (_currentUserId == null) return (false, "No user is logged in. Cannot perform adjustment.");
            if (AdjustmentProductId == 0) return (false, "Please select a product to adjust.");
            if (!int.TryParse(AdjustmentNewQuantityInput, out int actualNewQuantity) || actualNewQuantity < 0)
                return (false, "New quantity must be a valid non-negative number.");
            if (string.IsNullOrWhiteSpace(AdjustmentReason)) return (false, "Adjustment reason is required.");

            IsBusy = true; ErrorMessage = string.Empty;
            try
            {
                await _stockMovementService.RecordStockAdjustmentAsync(AdjustmentProductId, actualNewQuantity, AdjustmentReason, _currentUserId.Value);

                if (SelectedProductFilter?.Id == AdjustmentProductId)
                {
                    await LoadMovementsForProductAsync(AdjustmentProductId);
                }
                var productInList = AvailableProducts.FirstOrDefault(p => p.Id == AdjustmentProductId);
                if (productInList != null) productInList.QuantityInStock = actualNewQuantity;

                ClearAdjustmentForm();
                return (true, "Stock adjusted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform stock adjustment for Product ID {ProductId}", AdjustmentProductId);
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return (false, ErrorMessage);
            }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public void ClearAdjustmentForm()
        {
            AdjustmentProductId = 0;
            AdjustmentNewQuantityInput = "0";
            AdjustmentReason = string.Empty;
            ErrorMessage = string.Empty;
            // Notify UI that all form properties have been reset
            OnPropertyChanged(nameof(AdjustmentProductId));
            OnPropertyChanged(nameof(AdjustmentNewQuantityInput));
            OnPropertyChanged(nameof(AdjustmentReason));
            OnPropertyChanged(nameof(ErrorMessage));
        }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}