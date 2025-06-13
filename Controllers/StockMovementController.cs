// --- CORRECTED AND FINALIZED: Controllers/StockMovementController.cs ---
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace IMS_Group03.Controllers
{
    public class StockMovementController : INotifyPropertyChanged
    {
        private readonly IStockMovementService _stockMovementService;
        private readonly IProductService _productService;
        private readonly ILogger<StockMovementController> _logger; // IMPROVEMENT: Injected logger

        // FIX: Add state for the currently logged-in user
        private int? _currentUserId;

        #region Properties (Your code is mostly correct here)
        public ObservableCollection<StockMovement> StockMovements { get; } = new();
        public Product? SelectedProductFilter { get; set; } // Simplified for brevity
        public ObservableCollection<Product> AvailableProducts { get; } = new();

        // Form Properties for Manual Stock Adjustment
        public int AdjustmentProductId { get; set; }
        public string AdjustmentNewQuantityInput { get; set; } = "0";
        public string AdjustmentReason { get; set; } = string.Empty;

        // FIX: The "PerformedBy" string is removed, as it's now handled by the logged-in user ID.

        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        public StockMovementController(IStockMovementService stockMovementService, IProductService productService, ILogger<StockMovementController> logger)
        {
            _stockMovementService = stockMovementService;
            _productService = productService;
            _logger = logger;
            this.PropertyChanged += (s, e) => { }; // Simplified for brevity
        }

        // FIX: Method for the main application to set the current user
        public void SetCurrentUser(User user)
        {
            _currentUserId = user.Id;
        }

        // Your LoadInitialDataAsync and other loading methods are correct.

        public async Task<(bool Success, string Message)> PerformStockAdjustmentAsync()
        {
            // FIX: Check for logged-in user first.
            if (_currentUserId == null) return (false, "No user is logged in. Cannot perform adjustment.");

            if (AdjustmentProductId == 0) return (false, "Please select a product to adjust.");
            if (!int.TryParse(AdjustmentNewQuantityInput, out int actualNewQuantity) || actualNewQuantity < 0)
                return (false, "New quantity must be a valid non-negative number.");
            if (string.IsNullOrWhiteSpace(AdjustmentReason)) return (false, "Adjustment reason is required.");

            IsBusy = true; ErrorMessage = string.Empty;
            try
            {
                // FIX: Pass the currentUserId to the service call.
                await _stockMovementService.RecordStockAdjustmentAsync(
                    AdjustmentProductId,
                    actualNewQuantity,
                    AdjustmentReason,
                    _currentUserId.Value
                );

                // (Your logic to refresh the UI is excellent and preserved)
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
                // IMPROVEMENT: Use the injected logger
                _logger.LogError(ex, "Failed to perform stock adjustment for Product ID {ProductId}", AdjustmentProductId);
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return (false, ErrorMessage);
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        // Simplified OnPropertyChanged for brevity
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Other methods like LoadInitialDataAsync, LoadMovementsForProductAsync, ClearAdjustmentForm, etc.
        public async Task LoadInitialDataAsync() { /* ... */ }
        public async Task LoadMovementsForProductAsync(int productId) { /* ... */ }
        public void ClearAdjustmentForm() { /* ... */ }
    }
}