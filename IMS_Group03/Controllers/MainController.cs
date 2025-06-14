// --- THIS CODE IS ALREADY FINAL AND CORRECT ---
using IMS_Group03.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
// No need for System.Threading.Tasks as this controller is not async
using System.Windows;

namespace IMS_Group03.Controllers
{
    public class MainController : INotifyPropertyChanged
    {
        #region Properties (Your properties are perfect)
        private object _currentViewIdentifier = "DashboardView";
        public object CurrentViewIdentifier
        {
            get => _currentViewIdentifier;
            set { _currentViewIdentifier = value; OnPropertyChanged(); }
        }

        private string _statusMessage = "Application Ready.";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private bool _isGlobalBusy;
        public bool IsGlobalBusy
        {
            get => _isGlobalBusy;
            set { _isGlobalBusy = value; OnPropertyChanged(); }
        }

        private User? _currentUser;
        public User? CurrentUser
        {
            get => _currentUser;
            private set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WelcomeMessage));
            }
        }

        public string WelcomeMessage => CurrentUser != null ? $"Welcome, {CurrentUser.FullName}" : "Not Logged In";
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? OnLogoutRequested;

        // Constructor is empty and correct because it has no dependencies.
        public MainController()
        {
        }

        // This method correctly receives state from other controllers.
        // It does not perform database operations itself.
        public void SetAuthenticatedUser(User user)
        {
            CurrentUser = user;
            NavigateToDashboard();
        }

        #region Navigation Methods (These are correct)
        public void NavigateToDashboard()
        {
            CurrentViewIdentifier = "DashboardView";
            StatusMessage = "Dashboard";
        }
        // ... Other navigation methods are also correct ...
        public void NavigateToProducts()
        {
            CurrentViewIdentifier = "ProductsView";
            StatusMessage = "Managing Products";
        }

        public void NavigateToSuppliers()
        {
            CurrentViewIdentifier = "SuppliersView";
            StatusMessage = "Managing Suppliers";
        }

        public void NavigateToPurchaseOrders()
        {
            CurrentViewIdentifier = "PurchaseOrdersView";
            StatusMessage = "Managing Purchase Orders";
        }

        public void NavigateToStockMovements()
        {
            CurrentViewIdentifier = "StockMovementsView";
            StatusMessage = "Viewing Stock Movements";
        }

        public void NavigateToReports()
        {
            CurrentViewIdentifier = "ReportsView";
            StatusMessage = "Viewing Reports";
        }

        public void NavigateToUserSettings()
        {
            CurrentViewIdentifier = "UserSettingsView";
            StatusMessage = "User Settings";
        }
        #endregion

        // This method correctly manages logout state and events.
        public void Logout()
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CurrentUser = null;
                OnLogoutRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        // OnPropertyChanged implementation is correct.
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}