// --- COMPLETE AND FINALIZED: Controllers/MainController.cs ---
using IMS_Group03.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace IMS_Group03.Controllers
{
    public class MainController : INotifyPropertyChanged
    {
        #region Properties
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

        // Property to hold the authenticated user
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

        public MainController()
        {
            // Constructor is correct
        }

        // Method to be called by the Login process to set the user
        public void SetAuthenticatedUser(User user)
        {
            CurrentUser = user;
            // After setting the user, navigate to the initial screen
            NavigateToDashboard();
        }

        #region Navigation Methods (Your original methods restored)

        public void NavigateToDashboard()
        {
            CurrentViewIdentifier = "DashboardView";
            StatusMessage = "Dashboard";
        }

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

        // Method to handle user logout
        public void Logout()
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CurrentUser = null;
                // Raise the event that the MainWindow is listening for.
                OnLogoutRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}