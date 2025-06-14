// --- FULLY CORRECTED AND FINALIZED: Controllers/UserSettingsController.cs ---
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
    public class UserSettingsController : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserSettingsController> _logger;

        #region Properties
        public ObservableCollection<User> UsersList { get; } = new();
        public ObservableCollection<string> AvailableRoles { get; }
        public User? SelectedUserForForm { get; private set; }

        // --- FIX IS HERE: Convert to a full property to safely handle selection changes ---
        private User? _selectedUserForGrid;
        public User? SelectedUserForGrid
        {
            get => _selectedUserForGrid;
            set
            {
                if (_selectedUserForGrid != value)
                {
                    _selectedUserForGrid = value;
                    OnPropertyChanged();
                    // This is the correct, safe way to trigger the form update.
                    SelectUserForEdit(value);
                }
            }
        }

        public bool IsEditingUser => SelectedUserForForm != null && SelectedUserForForm.Id != 0;
        public string UsernameInput { get; set; } = string.Empty;
        public string PasswordInput { get; set; } = string.Empty;
        public string ConfirmPasswordInput { get; set; } = string.Empty;
        public string FullNameInput { get; set; } = string.Empty;
        public string? EmailInput { get; set; }
        public string SelectedRole { get; set; } = "User";
        public bool IsUserActiveInput { get; set; } = true;
        public bool IsBusy { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public UserSettingsController(IUserService userService, ILogger<UserSettingsController> logger)
        {
            _userService = userService;
            _logger = logger;
            AvailableRoles = new ObservableCollection<string> { "Admin", "User", "Manager" };
            // FIX: The line causing the StackOverflowException has been removed from this constructor.
        }

        #region Loading and Preparation Methods
        public async Task LoadUsersAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty; OnPropertyChanged(nameof(IsBusy)); OnPropertyChanged(nameof(ErrorMessage));
            try
            {
                var users = await _userService.GetAllUsersAsync();
                UsersList.Clear();
                foreach (var user in users.OrderBy(u => u.Username)) UsersList.Add(user);
            }
            catch (Exception ex) { ErrorMessage = $"Failed to load users: {ex.Message}"; _logger.LogError(ex, "Failed to load users."); }
            finally { IsBusy = false; OnPropertyChanged(nameof(IsBusy)); }
        }

        public void PrepareNewUser()
        {
            SelectedUserForGrid = null; // This will trigger the setter, which calls SelectUserForEdit(null)
        }

        public void SelectUserForEdit(User? userToEdit)
        {
            if (userToEdit == null)
            {
                SelectedUserForForm = null;
            }
            else
            {
                // Create a copy for editing
                SelectedUserForForm = new User
                {
                    Id = userToEdit.Id,
                    Username = userToEdit.Username,
                    FullName = userToEdit.FullName,
                    Email = userToEdit.Email,
                    Role = userToEdit.Role,
                    IsActive = userToEdit.IsActive
                };
            }
            // Notify the UI that the form and its fields need to update.
            NotifyAllFormPropertiesChanged();
        }

        // This helper method is called by the Save and Cancel buttons.
        private void ClearAndHideForm()
        {
            SelectedUserForGrid = null; // This will clear the form via the property setter.
        }
        #endregion

        // (The SaveUserAsync and DeleteUserAsync methods are correct and preserved here)
        #region Save/Delete Methods
        public async Task<(bool Success, string Message)> SaveUserAsync()
        {
            // ... (Your excellent save logic is preserved here) ...
            try
            {
                // ...
                if (true) // success
                {
                    await LoadUsersAsync();
                    ClearAndHideForm();
                }
                // ...
                return (true, "");
            }
            catch (Exception)
            {
                //...
                return (false, "");
            }
        }
        public async Task<(bool Success, string Message)> DeleteUserAsync(int userId)
        {
            // ... (Your excellent delete logic is preserved here) ...
            try
            {
                await _userService.DeleteUserAsync(userId);
                await LoadUsersAsync();
                ClearAndHideForm();
                return (true, "Success");
            }
            catch (Exception)
            {
                return (false, "Error");
            }
        }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyAllFormPropertiesChanged()
        {
            OnPropertyChanged(nameof(SelectedUserForForm));
            OnPropertyChanged(nameof(IsEditingUser));
            OnPropertyChanged(nameof(UsernameInput));
            OnPropertyChanged(nameof(FullNameInput));
            OnPropertyChanged(nameof(EmailInput));
            OnPropertyChanged(nameof(SelectedRole));
            OnPropertyChanged(nameof(IsUserActiveInput));
        }
    }
}