// ---Controllers/UserSettingsController.cs ---
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
    public class UserSettingsController : INotifyPropertyChanged
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UserSettingsController> _logger;

        #region Properties (This section is complete and correct)
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

        public UserSettingsController(IServiceScopeFactory scopeFactory, ILogger<UserSettingsController> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            AvailableRoles = new ObservableCollection<string> { "Admin", "User", "Manager" };

        }

        #region UI State Management Methods (Refactored to be safe)

        public void PrepareNewUser()
        {
            // This is the single entry point for creating a new user form.
            SelectUserForEdit(new User());
        }

        public void SelectUserForEdit(User? userToEdit)
        {
            
            IsBusy = false;
            ErrorMessage = string.Empty;

            SelectedUserForGrid = userToEdit;
            SelectedUserForForm = userToEdit;

            // The logic is moved here from the removed OnSelectedUserForFormChanged handler.
            if (SelectedUserForForm != null)
            {
                // Populate form fields from the model
                UsernameInput = SelectedUserForForm.Username;
                FullNameInput = SelectedUserForForm.FullName;
                EmailInput = SelectedUserForForm.Email;
                SelectedRole = SelectedUserForForm.Role ?? "User";
                IsUserActiveInput = SelectedUserForForm.IsActive;
                PasswordInput = string.Empty;
                ConfirmPasswordInput = string.Empty;
            }
            else
            {
                // Clear form fields if we are deselecting (userToEdit is null)
                ClearFormFields();
            }
            // Notify the UI of all potential changes at once.
            NotifyAllPropertiesChanged();
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
        }

        #endregion

        #region Database Operations (This logic is already correct)

        public async Task LoadUsersAsync()
        {
            IsBusy = true; ErrorMessage = string.Empty; NotifyAllPropertiesChanged();

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    var users = await userService.GetAllUsersAsync();
                    UsersList.Clear();
                    foreach (var user in users.OrderBy(u => u.Username)) UsersList.Add(user);
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Failed to load users. A database error occurred.";
                    _logger.LogError(ex, ErrorMessage);
                    NotifyAllPropertiesChanged();
                }
                finally { IsBusy = false; NotifyAllPropertiesChanged(); }
            }
        }

        public async Task<(bool Success, string Message)> SaveUserAsync()
        {
            ErrorMessage = string.Empty;
            if (SelectedUserForForm == null) return (false, "No user data to save.");
            if (string.IsNullOrWhiteSpace(UsernameInput)) return (false, "Username is required.");
           

            IsBusy = true; NotifyAllPropertiesChanged();

            using (var scope = _scopeFactory.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                try
                {
                    SelectedUserForForm.Username = UsernameInput;
                    SelectedUserForForm.FullName = FullNameInput;
                    SelectedUserForForm.Email = EmailInput;
                    SelectedUserForForm.Role = SelectedRole;
                    SelectedUserForForm.IsActive = IsUserActiveInput;

                    bool isNewUser = SelectedUserForForm.Id == 0;
                    (bool Success, string ErrorMessage) result;

                    if (isNewUser)
                    {
                        var createResult = await userService.CreateUserAsync(SelectedUserForForm, PasswordInput);
                        result = (createResult.Success, createResult.ErrorMessage);
                    }
                    else
                    {
                        var updateResult = await userService.UpdateUserAsync(SelectedUserForForm);
                        if (updateResult.Success && !string.IsNullOrWhiteSpace(PasswordInput))
                        {
                            var resetResult = await userService.AdminResetUserPasswordAsync(SelectedUserForForm.Id, PasswordInput);
                            if (!resetResult.Success) return (false, $"User updated, but password reset failed: {resetResult.ErrorMessage}");
                        }
                        result = updateResult;
                    }

                    if (result.Success)
                    {
                        await LoadUsersAsync();
                        SelectUserForEdit(null); // Safely clear and hide the form
                    }
                    else
                    {
                        ErrorMessage = result.ErrorMessage;
                        NotifyAllPropertiesChanged();
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while saving user {Username}", UsernameInput);
                    ErrorMessage = "An unexpected system error occurred.";
                    NotifyAllPropertiesChanged();
                    return (false, ErrorMessage);
                }
                finally { IsBusy = false; NotifyAllPropertiesChanged(); }
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int userId)
        {
            IsBusy = true; NotifyAllPropertiesChanged();
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    var result = await userService.DeleteUserAsync(userId);
                if (result.Success)
                    {
                        await LoadUsersAsync();
                        SelectUserForEdit(null);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting user {UserId}", userId);
                    return (false, "An unexpected error occurred.");
                }
                finally { IsBusy = false; NotifyAllPropertiesChanged(); }
            }
        }

        #endregion

        #region PropertyChanged Notification
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(SelectedUserForForm));
            OnPropertyChanged(nameof(SelectedUserForGrid));
            OnPropertyChanged(nameof(IsEditingUser));
            OnPropertyChanged(nameof(UsernameInput));
            OnPropertyChanged(nameof(FullNameInput));
            OnPropertyChanged(nameof(EmailInput));
            OnPropertyChanged(nameof(SelectedRole));
            OnPropertyChanged(nameof(IsUserActiveInput));
            OnPropertyChanged(nameof(IsBusy));
            OnPropertyChanged(nameof(ErrorMessage));
        }
        #endregion
    }
}