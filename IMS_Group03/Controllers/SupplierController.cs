// --- Controllers/SupplierController.cs ---
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
    public class SupplierController : INotifyPropertyChanged
    {
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SupplierController> _logger;

        #region Properties (Your existing properties are perfect and unchanged)
        public ObservableCollection<Supplier> Suppliers { get; } = new ObservableCollection<Supplier>();

        private Supplier? _selectedSupplierForForm;
        public Supplier? SelectedSupplierForForm
        {
            get => _selectedSupplierForForm;
            set { _selectedSupplierForForm = value; OnPropertyChanged(); }
        }

        private Supplier? _selectedSupplierGridItem;
        public Supplier? SelectedSupplierGridItem
        {
            get => _selectedSupplierGridItem;
            set
            {
                if (_selectedSupplierGridItem != value)
                {
                    _selectedSupplierGridItem = value;
                    OnPropertyChanged();
                    // If you want selecting the grid to auto-populate the form:
                    PrepareSupplierForEdit(_selectedSupplierGridItem);
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set { _isBusy = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            private set { _errorMessage = value ?? string.Empty; OnPropertyChanged(); }
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        
        public SupplierController(IServiceScopeFactory scopeFactory, ILogger<SupplierController> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        
        public async Task LoadSuppliersAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty; OnAllPropertiesChanged();

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();
                    var supplierModels = await supplierService.GetAllSuppliersAsync();

                    Suppliers.Clear();
                    if (supplierModels != null)
                    {
                        foreach (var model in supplierModels.OrderBy(s => s.Name))
                        {
                            Suppliers.Add(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load suppliers.");
                    ErrorMessage = "Failed to load suppliers. A database error occurred.";
                }
                finally
                {
                    IsBusy = false; OnAllPropertiesChanged();
                }
            }
        }

        #region UI-Only Methods (Correct and unchanged)
        public void PrepareNewSupplier()
        {
            ErrorMessage = string.Empty;
            SelectedSupplierGridItem = null;
            SelectedSupplierForForm = new Supplier();
            OnAllPropertiesChanged();
        }

        public void PrepareSupplierForEdit(Supplier? supplierToEdit)
        {
            ErrorMessage = string.Empty;
            if (supplierToEdit == null)
            {
                SelectedSupplierForForm = null;
            }
            else
            {
                SelectedSupplierForForm = new Supplier
                {
                    Id = supplierToEdit.Id,
                    Name = supplierToEdit.Name ?? string.Empty,
                    ContactPerson = supplierToEdit.ContactPerson ?? string.Empty,
                    Email = supplierToEdit.Email ?? string.Empty,
                    Phone = supplierToEdit.Phone ?? string.Empty,
                    Address = supplierToEdit.Address ?? string.Empty
                };
            }
            OnAllPropertiesChanged();
        }

        public void ClearFormSelection()
        {
            SelectedSupplierForForm = null;
            SelectedSupplierGridItem = null;
            ErrorMessage = string.Empty;
            OnAllPropertiesChanged();
        }
        #endregion

        #region Database Write Methods (Now using Scopes)

        public async Task<(bool Success, string Message)> SaveSupplierAsync()
        {
            if (SelectedSupplierForForm == null) return (false, "No supplier data to save.");
            if (string.IsNullOrWhiteSpace(SelectedSupplierForForm.Name)) return (false, "Supplier name is required.");
      

            IsBusy = true; ErrorMessage = string.Empty; OnAllPropertiesChanged();

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();
                    bool isNewSupplier = SelectedSupplierForForm.Id == 0;

                    if (!await supplierService.IsSupplierNameUniqueAsync(SelectedSupplierForForm.Name, isNewSupplier ? (int?)null : SelectedSupplierForForm.Id))
                    {
                        return (false, $"Supplier name '{SelectedSupplierForForm.Name}' already exists.");
                    }

                    if (isNewSupplier)
                    {
                        await supplierService.AddSupplierAsync(SelectedSupplierForForm);
                    }
                    else
                    {
                        await supplierService.UpdateSupplierAsync(SelectedSupplierForForm);
                    }

                    await LoadSuppliersAsync(); // Reloads data with its own new scope
                    ClearFormSelection();
                    return (true, "Supplier saved successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save supplier.");
                    ErrorMessage = $"An unexpected error occurred during save: {ex.Message}";
                    return (false, ErrorMessage);
                }
                finally
                {
                    IsBusy = false; OnAllPropertiesChanged();
                }
            }
        }

        public async Task<(bool Success, string Message)> DeleteSupplierAsync(int supplierId)
        {
            IsBusy = true; ErrorMessage = string.Empty; OnAllPropertiesChanged();

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();
                    await supplierService.DeleteSupplierAsync(supplierId);

                    await LoadSuppliersAsync();
                    ClearFormSelection();
                    return (true, "Supplier deleted successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete supplier {SupplierId}", supplierId);
                    ErrorMessage = $"Delete failed. The supplier may have products assigned to it.";
                    return (false, ErrorMessage);
                }
                finally
                {
                    IsBusy = false; OnAllPropertiesChanged();
                }
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
            OnPropertyChanged(nameof(SelectedSupplierForForm));
            OnPropertyChanged(nameof(SelectedSupplierGridItem));
        }
    }
}