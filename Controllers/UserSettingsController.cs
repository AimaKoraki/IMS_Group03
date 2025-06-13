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

        #region Properties (Your excellent structure is preserved)
        public ObservableCollection<User> UsersList { get; } = new();
        public ObservableCollection<string> AvailableRoles { get; }
        public User? SelectedUserForForm { get; private set; }
        public User? SelectedUserForGrid { get; set; }
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
            this.PropertyChanged += OnSelectedUserForFormChanged;
        }

        // This handler correctly populates the form when a user is selected for editing
        private void OnSelectedUserForFormChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedUserForForm)) return;

            if (SelectedUserForForm != null)
            {
                UsernameInput = SelectedUserForForm.Username;
                FullNameInput = SelectedUserForForm.FullName;
                EmailInput = SelectedUserForForm.Email;
                SelectedRole = SelectedUserForForm.Role;
                IsUserActiveInput = SelectedUserForForm.IsActive;
                PasswordInput = string.Empty;
                ConfirmPasswordInput = string.Empty;
            }
            else
            {
                ClearFormFields();
            }
            NotifyAllPropertiesChanged();
        }

        public async Task LoadUsersAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty;
            NotifyAllPropertiesChanged();
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
            SelectedUserForGrid = null;
            SelectedUserForForm = new User(); // The setter will trigger OnSelectedUserForFormChanged
            OnPropertyChanged(nameof(SelectedUserForGrid));
            OnPropertyChanged(nameof(SelectedUserForForm));
        }

        public void SelectUserForEdit(User? userToEdit)
        {
            SelectedUserForGrid = userToEdit;
            SelectedUserForForm = userToEdit;
            OnPropertyChanged(nameof(SelectedUserForGrid));
            OnPropertyChanged(nameof(SelectedUserForForm));
        }

        private void ClearFormFields()
        {
            UsernameInput = string.Empty;
            FullNameInput = string.Empty;
            EmailInput = null;
            PasswordInput = string.Empty;
            ConfirmPasswordInput = string.Empty;
            SelectedRole = "User";
            IsUserActiveInput = true;
            ErrorMessage = string.Empty;
        }

        public async Task<(bool Success, string Message)> SaveUserAsync()
        {
            ErrorMessage = string.Empty;
            if (SelectedUserForForm == null) return (false, "No user data to save.");
            if (string.IsNullOrWhiteSpace(UsernameInput)) return (false, "Username is required.");

            // Your excellent validation logic is preserved here
            bool isNewUser = !IsEditingUser;
            if (isNewUser)
            {
                if (string.IsNullOrWhiteSpace(PasswordInput)) return (false, "Password is required for new users.");
                if (PasswordInput != ConfirmPasswordInput) return (false, "Passwords do not match.");
            }
            else if (!string.IsNullOrWhiteSpace(PasswordInput) && PasswordInput != ConfirmPasswordInput)
            {
                return (false, "Passwords do not match.");
            }

            // Populate the model from the form fields
            SelectedUserForForm.Username = UsernameInput;
            SelectedUserForForm.FullName = FullNameInput;
            SelectedUserForForm.Email = EmailInput;
            SelectedUserForForm.Role = SelectedRole;
            SelectedUserForForm.IsActive = IsUserActiveInput;

            IsBusy = true; OnPropertyChanged(nameof(IsBusy));
            try
            {
                if (isNewUser)
                {
                    // Correctly deconstruct the 3-part tuple from the service
                    var (success, createdUser, message) = await _userService.CreateUserAsync(SelectedUserForForm, PasswordInput);
                    if (!success) ErrorMessage = message;
                    // Return the 2-part tuple that this method requires
                    return (success, message);
                }
                else // Editing existing user
                {
                    var (updateSuccess, updateMessage) = await _userService.UpdateUserAsync(SelectedUserForForm);
                    if (updateSuccess && !string.IsNullOrWhiteSpace(PasswordInput))
                    {
                        var (resetSuccess, resetMessage) = await _userService.AdminResetUserPasswordAsync(SelectedUserForForm.Id, PasswordInput);
                        if (!resetSuccess)
                        {
                            // Combine error messages
                            return (false, $"{updateMessage} But password reset failed: {resetMessage}");
                        }
                    }
                    if (!updateSuccess) ErrorMessage = updateMessage;
                    return (updateSuccess, updateMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving user {Username}", UsernameInput);
                ErrorMessage = "An unexpected error occurred while saving the user.";
                return (false, ErrorMessage);
            }
            finally
            {
                // On success, this will run after the return
                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    await LoadUsersAsync();
                    SelectUserForEdit(null); // Clear the form
                }
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int userId)
        {
            IsBusy = true; OnPropertyChanged(nameof(IsBusy));
            try
            {
                return await _userService.DeleteUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return (false, "An unexpected error occurred.");
            }
            finally
            {
                await LoadUsersAsync();
                SelectUserForEdit(null);
                IsBusy = false; OnPropertyChanged(nameof(IsBusy));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(SelectedUserForForm));
            OnPropertyChanged(nameof(IsEditingUser));
            OnPropertyChanged(nameof(UsernameInput));
            OnPropertyChanged(nameof(PasswordInput));
            OnPropertyChanged(nameof(ConfirmPasswordInput));
            OnPropertyChanged(nameof(FullNameInput));
            OnPropertyChanged(nameof(EmailInput));
            OnPropertyChanged(nameof(SelectedRole));
            OnPropertyChanged(nameof(IsUserActiveInput));
            OnPropertyChanged(nameof(IsBusy));
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }
}