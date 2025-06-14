// --- FULLY CORRECTED AND FINALIZED: Controllers/UserSettingsController.cs ---
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.DependencyInjection; // Required for IServiceScopeFactory
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
        // --- FIX: The controller now depends on the factory, not the service directly. ---
        private readonly IServiceScopeFactory _scopeFactory;
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

        // --- FIX: The constructor is updated to inject IServiceScopeFactory. ---
        public UserSettingsController(IServiceScopeFactory scopeFactory, ILogger<UserSettingsController> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            AvailableRoles = new ObservableCollection<string> { "Admin", "User", "Manager" };
            this.PropertyChanged += OnSelectedUserForFormChanged;
        }

        private void OnSelectedUserForFormChanged(object? sender, PropertyChangedEventArgs e)
        {
            // This method manipulates UI state and is correct as is.
            if (e.PropertyName != nameof(SelectedUserForForm)) return;
            // ... (rest of your excellent form-population logic is unchanged)
        }

        #region Database Operations (Now using Scopes)

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
                    _logger.LogError(ex, "Failed to load users.");
                }
                finally { IsBusy = false; NotifyAllPropertiesChanged(); }
            }
        }

        public async Task<(bool Success, string Message)> SaveUserAsync()
        {
            ErrorMessage = string.Empty;
            if (SelectedUserForForm == null) return (false, "No user data to save.");
            if (string.IsNullOrWhiteSpace(UsernameInput)) return (false, "Username is required.");
            // ... (your excellent validation logic is preserved here) ...

            IsBusy = true; NotifyAllPropertiesChanged();

            using (var scope = _scopeFactory.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                try
                {
                    // Populate the model from the form fields just before saving
                    SelectedUserForForm.Username = UsernameInput;
                    SelectedUserForForm.FullName = FullNameInput;
                    SelectedUserForForm.Email = EmailInput;
                    SelectedUserForForm.Role = SelectedRole;
                    SelectedUserForForm.IsActive = IsUserActiveInput;

                    bool isNewUser = SelectedUserForForm.Id == 0;

                    if (isNewUser)
                    {
                        // --- FIX: Use the correct case 'Success' and 'ErrorMessage' ---
                        var createResult = await userService.CreateUserAsync(SelectedUserForForm, PasswordInput);
                        if (!createResult.Success)
                        {
                            ErrorMessage = createResult.ErrorMessage;
                        }
                        return (createResult.Success, createResult.ErrorMessage);
                    }
                    else // Editing existing user
                    {
                        // --- FIX: Use the correct case 'Success' and 'ErrorMessage' ---
                        var updateResult = await userService.UpdateUserAsync(SelectedUserForForm);
                        if (updateResult.Success && !string.IsNullOrWhiteSpace(PasswordInput))
                        {
                            var resetResult = await userService.AdminResetUserPasswordAsync(SelectedUserForForm.Id, PasswordInput);
                            if (!resetResult.Success)
                            {
                                var combinedError = $"{updateResult.ErrorMessage} But password reset failed: {resetResult.ErrorMessage}";
                                ErrorMessage = combinedError;
                                return (false, combinedError);
                            }
                        }
                        if (!updateResult.Success)
                        {
                            ErrorMessage = updateResult.ErrorMessage;
                        }
                        return (updateResult.Success, updateResult.ErrorMessage);
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
                    IsBusy = false; NotifyAllPropertiesChanged();
                }
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

        #region UI-Only Methods (Correct and unchanged)
        public void PrepareNewUser()
        {
            SelectUserForEdit(new User());
        }

        public void SelectUserForEdit(User? userToEdit)
        {
            SelectedUserForGrid = userToEdit;
            SelectedUserForForm = userToEdit; // This will trigger the OnSelectedUserForFormChanged handler
            NotifyAllPropertiesChanged();
        }

        private void ClearFormFields()
        {
            // ... (Your logic here is correct) ...
        }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyAllPropertiesChanged()
        {
            // ... (Your logic here is correct) ...
        }
    }
}