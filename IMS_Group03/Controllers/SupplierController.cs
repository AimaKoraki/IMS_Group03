// --- FULLY CORRECTED AND FINALIZED: Controllers/SupplierController.cs ---
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
    public class SupplierController : INotifyPropertyChanged
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SupplierController> _logger;
        private int? _currentUserId;

        #region Properties
        public ObservableCollection<Supplier> Suppliers { get; } = new();
        public Supplier? SelectedSupplierForForm { get; private set; }
        public Supplier? SelectedSupplierGridItem { get; set; }
        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public SupplierController(ISupplierService supplierService, ILogger<SupplierController> logger)
        {
            _supplierService = supplierService;
            _logger = logger;
        }

        public void SetCurrentUser(User user)
        {
            _currentUserId = user.Id;
        }

        #region Loading and Preparation Methods
        public async Task LoadSuppliersAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage));
            try
            {
                var supplierModels = await _supplierService.GetAllSuppliersAsync();
                Suppliers.Clear();
                foreach (var model in supplierModels.OrderBy(s => s.Name))
                {
                    Suppliers.Add(model);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load suppliers.";
                _logger.LogError(ex, ErrorMessage);
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public void PrepareNewSupplier()
        {
            SelectedSupplierGridItem = null;
            SelectedSupplierForForm = new Supplier();
            OnPropertyChanged(nameof(SelectedSupplierGridItem));
            OnPropertyChanged(nameof(SelectedSupplierForForm));
        }

        public void PrepareSupplierForEdit(Supplier? supplierToEdit)
        {
            if (supplierToEdit == null)
            {
                SelectedSupplierForForm = null;
            }
            else
            {
                SelectedSupplierForForm = new Supplier // Create a copy for editing
                {
                    Id = supplierToEdit.Id,
                    Name = supplierToEdit.Name,
                    ContactPerson = supplierToEdit.ContactPerson,
                    Email = supplierToEdit.Email,
                    Phone = supplierToEdit.Phone,
                    Address = supplierToEdit.Address
                };
            }
            OnPropertyChanged(nameof(SelectedSupplierForForm));
        }

        public void ClearFormSelection()
        {
            SelectedSupplierForForm = null;
            OnPropertyChanged(nameof(SelectedSupplierForForm));
        }
        #endregion

        #region Save/Delete Methods
        public async Task<(bool Success, string Message)> SaveSupplierAsync()
        {
            if (SelectedSupplierForForm == null) return (false, "No supplier data to save.");
            if (string.IsNullOrWhiteSpace(SelectedSupplierForForm.Name)) return (false, "Supplier name is required.");

            IsBusy = true; ErrorMessage = string.Empty;
            try
            {
                if (SelectedSupplierForForm.Id == 0)
                {
                    await _supplierService.AddSupplierAsync(SelectedSupplierForForm);
                }
                else
                {
                    await _supplierService.UpdateSupplierAsync(SelectedSupplierForForm);
                }

                await LoadSuppliersAsync();
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

        public async Task<(bool Success, string Message)> DeleteSupplierAsync(int supplierId)
        {
            IsBusy = true; ErrorMessage = string.Empty;
            try
            {
                await _supplierService.DeleteSupplierAsync(supplierId);
                await LoadSuppliersAsync();
                ClearFormSelection();
                return (true, "Supplier deleted successfully.");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Delete failed: {ex.Message}";
                _logger.LogError(ex, "Failed to delete supplier with ID {SupplierId}", supplierId);
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