// --- Controllers/MainController.cs ---
using IMS_Group03.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace IMS_Group03.Controllers
{
    public class MainController : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        public INotifyPropertyChanged? CurrentPageController { get; private set; }

        private User? _currentUser;
        public User? CurrentUser
        {
            get => _currentUser;
            private set
            {
                // This custom setter is the key.
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged(); // Notify that CurrentUser itself has changed
                    OnPropertyChanged(nameof(WelcomeMessage)); // Notify that the welcome message needs to update
                    OnPropertyChanged(nameof(IsCurrentUserAdmin)); // Notify that admin access needs to update
                }
            }
        }

        public bool IsCurrentUserAdmin => CurrentUser?.Role == "Admin";
        public string WelcomeMessage => CurrentUser != null ? $"Welcome, {CurrentUser.FullName}" : "Not Logged In";
        public string StatusMessage { get; set; } = "Application Ready.";

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? OnLogoutRequested;

        public MainController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private void NavigateTo<TController>() where TController : INotifyPropertyChanged
        {
            if (CurrentUser == null) return;

            var controller = _serviceProvider.GetRequiredService<TController>();

            var setUserMethod = controller.GetType().GetMethod("SetCurrentUser");
            if (setUserMethod != null)
            {
                setUserMethod.Invoke(controller, new object[] { CurrentUser });
            }

            CurrentPageController = controller;
            OnPropertyChanged(nameof(CurrentPageController));
        }

        // --- All navigation methods are correct ---
        public void NavigateToDashboard() { StatusMessage = "Dashboard"; NavigateTo<DashboardController>(); }
        public void NavigateToProducts() { StatusMessage = "Managing Products"; NavigateTo<ProductController>(); }
        public void NavigateToSuppliers() { StatusMessage = "Managing Suppliers"; NavigateTo<SupplierController>(); }
        public void NavigateToPurchaseOrders() { StatusMessage = "Managing Purchase Orders"; NavigateTo<PurchaseOrderController>(); }
        public void NavigateToStockMovements() { StatusMessage = "Viewing Stock Movements"; NavigateTo<StockMovementController>(); }
        public void NavigateToReports() { StatusMessage = "Viewing Reports"; NavigateTo<ReportController>(); }
        public void NavigateToUserSettings() { if (!IsCurrentUserAdmin) return; StatusMessage = "User Settings"; NavigateTo<UserSettingsController>(); }

        #region Session Management Methods (With Final Fix)

        // --- FIX: The logic here is re-ordered to be 100% safe. ---
        public void SetAuthenticatedUser(User user)
        {
            // Step 1: Set the CurrentUser. This triggers all the OnPropertyChanged events.
            CurrentUser = user;

            // Step 2: Only AFTER the user is set and all notifications have been sent,
            // then we perform the navigation.
            NavigateToDashboard();
        }

        public void Logout()
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CurrentUser = null;
                CurrentPageController = null;
                OnPropertyChanged(nameof(CurrentPageController));
                OnLogoutRequested?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}