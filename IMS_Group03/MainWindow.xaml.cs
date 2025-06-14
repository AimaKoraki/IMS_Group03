// --- COMPLETE AND FINALIZED: MainWindow.xaml.cs ---
using IMS_Group03.Controllers;
using IMS_Group03.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Windows;

namespace IMS_Group03
{
    public partial class MainWindow : Window
    {
        private readonly MainController _mainController;

        public MainWindow()
        {
            InitializeComponent();

            if (App.ServiceProvider == null)
            {
                MessageBox.Show("Fatal Error: Service Provider is not initialized.", "Startup Error");
                Application.Current.Shutdown();
                return;
            }

            _mainController = App.ServiceProvider.GetRequiredService<MainController>();
            this.DataContext = _mainController;

            _mainController.PropertyChanged += MainController_PropertyChanged;
            _mainController.OnLogoutRequested += MainController_OnLogoutRequested;

            NavigateFrameToView(_mainController.CurrentViewIdentifier);
        }

        private void MainController_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainController.CurrentViewIdentifier))
            {
                NavigateFrameToView(_mainController.CurrentViewIdentifier);
            }
        }

        private void NavigateFrameToView(object viewIdentifier)
        {
            if (viewIdentifier is not string viewNameKey) return;

            // FIX: All views are now included in the navigation logic.
            Type? viewType = viewNameKey switch
            {
                "DashboardView" => typeof(DashboardView),
                "ProductsView" => typeof(ProductView),
                "SuppliersView" => typeof(SupplierView),
                "PurchaseOrdersView" => typeof(PurchaseOrderView),
                "StockMovementsView" => typeof(StockMovementView),
                "ReportsView" => typeof(ReportView),
                "UserSettingsView" => typeof(UserSettingsView),
                _ => null
            };

            if (viewType != null && MainFrame.Content?.GetType() != viewType)
            {
                var viewInstance = App.ServiceProvider.GetRequiredService(viewType);
                MainFrame.Content = viewInstance;
            }
        }

        #region Sidebar Button Clicks

        private void DashboardNavButton_Click(object sender, RoutedEventArgs e) => _mainController.NavigateToDashboard();
        private void ProductsNavButton_Click(object sender, RoutedEventArgs e) => _mainController.NavigateToProducts();
        private void SuppliersNavButton_Click(object sender, RoutedEventArgs e) => _mainController.NavigateToSuppliers();

        // FIX: All missing click handlers are now present.
        private void PurchaseOrdersNavButton_Click(object sender, RoutedEventArgs e) => _mainController.NavigateToPurchaseOrders();
        private void StockMovementsNavButton_Click(object sender, RoutedEventArgs e) => _mainController.NavigateToStockMovements();
        private void ReportsNavButton_Click(object sender, RoutedEventArgs e) => _mainController.NavigateToReports();
        private void UserSettingsNavButton_Click(object sender, RoutedEventArgs e) => _mainController.NavigateToUserSettings();

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController.Logout();
        }

        #endregion

        private void MainController_OnLogoutRequested(object? sender, EventArgs e)
        {
            var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();

            // Unsubscribe from events to prevent memory leaks before closing.
            if (_mainController != null)
            {
                _mainController.PropertyChanged -= MainController_PropertyChanged;
                _mainController.OnLogoutRequested -= MainController_OnLogoutRequested;
            }

            this.Close();
        }
    }
}