// Controllers/SupplierController.cs
using IMS_Group03.Models;
using IMS_Group03.Services;
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

        private ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            private set { _suppliers = value; OnPropertyChanged(); }
        }

        private Supplier? _selectedSupplierForForm;
        public Supplier? SelectedSupplierForForm
        {
            get => _selectedSupplierForForm;
            set { _selectedSupplierForForm = value; OnPropertyChanged(); }
        }

        // ***** ADD THIS PROPERTY *****
        private Supplier? _selectedSupplierGridItem; // Backing field (nullable)
        public Supplier? SelectedSupplierGridItem    // Public property for DataGrid's SelectedItem
        {
            get => _selectedSupplierGridItem;
            set
            {
                if (_selectedSupplierGridItem != value)
                {
                    _selectedSupplierGridItem = value;
                    OnPropertyChanged();
                    // Optional: If selecting an item in the grid should also prepare the edit form
                    // if (_selectedSupplierGridItem != null)
                    // {
                    //    PrepareSupplierForEdit(_selectedSupplierGridItem);
                    // }
                }
            }
        }
        // ***** END OF ADDED PROPERTY *****

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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        }

        public async Task LoadSuppliersAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var supplierModels = await _supplierService.GetAllSuppliersAsync();
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
                Console.WriteLine($"Error loading suppliers: {ex.Message} {ex.StackTrace}");
                ErrorMessage = $"Failed to load suppliers: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void PrepareNewSupplier()
        {
            ErrorMessage = string.Empty;
            SelectedSupplierGridItem = null; // Deselect from grid if user clicks "Add New"
            SelectedSupplierForForm = new Supplier(); // Assumes Supplier model initializes its strings
        }

        public void PrepareSupplierForEdit(Supplier? supplierToEdit)
        {
            ErrorMessage = string.Empty;
            if (supplierToEdit == null)
            {
                SelectedSupplierForForm = null;
                return;
            }
            SelectedSupplierGridItem = supplierToEdit; // Keep grid selection in sync
            SelectedSupplierForForm = new Supplier // Create a copy for editing
            {
                Id = supplierToEdit.Id,
                Name = supplierToEdit.Name ?? string.Empty,
                ContactPerson = supplierToEdit.ContactPerson ?? string.Empty,
                Email = supplierToEdit.Email ?? string.Empty,
                Phone = supplierToEdit.Phone ?? string.Empty,
                Address = supplierToEdit.Address ?? string.Empty
                // DateCreated and LastUpdated are usually not part of the edit form directly for suppliers
            };
        }

        public async Task<(bool Success, string Message)> SaveSupplierAsync()
        {
            if (SelectedSupplierForForm == null)
            {
                return (false, "No supplier data to save.");
            }

            if (string.IsNullOrWhiteSpace(SelectedSupplierForForm.Name))
                return (false, "Supplier name is required.");
            if (!string.IsNullOrWhiteSpace(SelectedSupplierForForm.Email) && !(SelectedSupplierForForm.Email.Contains("@") && SelectedSupplierForForm.Email.Contains(".")))
                return (false, "Invalid email format.");

            bool isNewSupplier = SelectedSupplierForForm.Id == 0;
            if (SelectedSupplierForForm.Name == null) return (false, "Supplier Name cannot be null for uniqueness check.");

            if (!await _supplierService.IsSupplierNameUniqueAsync(SelectedSupplierForForm.Name, isNewSupplier ? (int?)null : SelectedSupplierForForm.Id))
            {
                return (false, $"Supplier name '{SelectedSupplierForForm.Name}' already exists.");
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            string successMessage;
            try
            {
                if (isNewSupplier)
                {
                    await _supplierService.AddSupplierAsync(SelectedSupplierForForm);
                    successMessage = "Supplier added successfully.";
                }
                else
                {
                    await _supplierService.UpdateSupplierAsync(SelectedSupplierForForm);
                    successMessage = "Supplier updated successfully.";
                }
                await LoadSuppliersAsync();
                ClearFormSelection(); // Clears both SelectedSupplierForForm and SelectedSupplierGridItem
                return (true, successMessage);
            }
            catch (InvalidOperationException ioEx) { ErrorMessage = ioEx.Message; return (false, ioEx.Message); }
            catch (ArgumentException argEx) { ErrorMessage = argEx.Message; return (false, argEx.Message); }
            catch (KeyNotFoundException kfEx) { ErrorMessage = kfEx.Message; return (false, kfEx.Message); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving supplier: {ex.Message} {ex.StackTrace}");
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return (false, ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<(bool Success, string Message)> DeleteSupplierAsync(int supplierId)
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                await _supplierService.DeleteSupplierAsync(supplierId);
                await LoadSuppliersAsync();
                ClearFormSelection(); // Clear selection if deleted item was selected
                return (true, "Supplier deleted successfully.");
            }
            catch (InvalidOperationException ioEx) { ErrorMessage = ioEx.Message; return (false, ioEx.Message); }
            catch (KeyNotFoundException) { ErrorMessage = "Supplier not found for deletion."; return (false, ErrorMessage); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting supplier: {ex.Message} {ex.StackTrace}");
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return (false, ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ClearFormSelection()
        {
            SelectedSupplierForForm = null;
            SelectedSupplierGridItem = null; // Also clear this
            ErrorMessage = string.Empty;
        }
    }
}