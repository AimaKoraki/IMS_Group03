// --- FULLY CORRECTED AND FINALIZED: MainWindow.xaml.cs ---
using IMS_Group03.Controllers;
using IMS_Group03.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace IMS_Group03
{
    public partial class MainWindow : Window
    {
        private readonly MainController _mainController;

        public MainWindow(MainController mainController)
        {
            InitializeComponent();
            _mainController = mainController;
            this.DataContext = _mainController;

            // Subscribe to the LogoutRequested event from the MainController
            _mainController.OnLogoutRequested += MainController_OnLogoutRequested;
            // Ensure we unsubscribe when the window closes to prevent memory leaks
            this.Closing += MainWindow_Closing;
        }

        // --- NEW: Event Handlers to manage window lifetime ---
        private void MainController_OnLogoutRequested(object? sender, EventArgs e)
        {
            // When the MainController signals a logout...
            var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show(); // Show the login window
            this.Close(); // Close this MainWindow
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            // Clean up the event subscription when the window is closed
            if (_mainController != null)
            {
                _mainController.OnLogoutRequested -= MainController_OnLogoutRequested;
            }
        }

        // --- UPDATED: All navigation click handlers now simply call the MainController ---
        // They no longer need to know about the Frame or which View to navigate to.

        private void DashboardNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController.NavigateToDashboard();
        }

        private void ProductsNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController.NavigateToProducts();
        }

        private void SuppliersNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController.NavigateToSuppliers();
        }

        private void PurchaseOrdersNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController.NavigateToPurchaseOrders();
        }

        private void StockMovementsNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController.NavigateToStockMovements();
        }

        private void ReportsNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController.NavigateToReports();
        }

        private void UserSettingsNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController.NavigateToUserSettings();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController.Logout();
        }
    }
}