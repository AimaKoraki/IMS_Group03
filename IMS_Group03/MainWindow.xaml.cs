// MainWindow.xaml.cs
using IMS_Group03.Controllers;
using IMS_Group03.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IMS_Group03
{
    public partial class MainWindow : Window
    {
        private readonly MainController? _mainController;
        private Button? _currentSelectedNavButton = null;

        public MainWindow()
        {
            InitializeComponent();

            if (App.ServiceProvider == null)
            {
                MessageBox.Show("Critical Error: ServiceProvider is not initialized...", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            _mainController = App.ServiceProvider.GetService<MainController>();
            if (_mainController == null)
            {
                MessageBox.Show("Critical Error: MainController could not be resolved...", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.IsEnabled = false;
                return;
            }
            this.DataContext = _mainController;
            _mainController.PropertyChanged += MainController_PropertyChanged;
            this.Loaded += MainWindow_Loaded_Async;
        }

        private async void MainWindow_Loaded_Async(object sender, RoutedEventArgs e)
        {
            if (_mainController != null)
            {
                try
                {
                    await _mainController.InitializeAsync();
                    if (_mainController.CurrentViewIdentifier as string == "DashboardView")
                    {
                        UpdateSelectedNavButton(DashboardNavButton); // Assumes DashboardNavButton is x:Name in XAML
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during MainController initialization: {ex.Message}");
                    MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MainController_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainController.CurrentViewIdentifier) && _mainController != null)
            {
                NavigateFrameToView(_mainController.CurrentViewIdentifier);
            }
        }

        private void NavigateFrameToView(object? viewIdentifier)
        {
            if (viewIdentifier is string viewNameKey)
            {
                Type? viewType = null;
                switch (viewNameKey)
                {
                    case "DashboardView": viewType = typeof(Views.DashboardView); break;
                    case "ProductsView": viewType = typeof(Views.ProductView); break;
                    case "SuppliersView": viewType = typeof(Views.SupplierView); break;
                    case "PurchaseOrdersView": viewType = typeof(Views.PurchaseOrderView); break;
                    case "StockMovementsView": viewType = typeof(Views.StockMovementView); break;
                    case "ReportsView": viewType = typeof(Views.ReportView); break;
                    case "UserSettingsView": viewType = typeof(Views.UserSettingsView); break; // Ensure UserSettingsView exists
                }

                if (viewType != null)
                {
                    if (MainFrame == null || MainFrame.Content?.GetType() == viewType) return;
                    try
                    {
                        UIElement? viewInstance = App.ServiceProvider?.GetService(viewType) as UIElement;
                        if (viewInstance != null)
                        {
                            MainFrame.Content = viewInstance;
                            UpdateWindowTitle(viewType);
                        }
                        else { /* ... error handling ... */ }
                    }
                    catch (Exception ex) { /* ... error handling ... */ }
                }
                else { Debug.WriteLine($"Unknown view identifier: {viewNameKey}"); }
            }
        }

        // --- Sidebar button clicks ---
        private void DashboardNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController?.NavigateToDashboard();
            UpdateSelectedNavButton(sender as Button);
        }
        private void ProductsNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController?.NavigateToProducts();
            UpdateSelectedNavButton(sender as Button);
        }
        private void SuppliersNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController?.NavigateToSuppliers();
            UpdateSelectedNavButton(sender as Button);
        }
        private void PurchaseOrdersNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController?.NavigateToPurchaseOrders();
            UpdateSelectedNavButton(sender as Button);
        }
        private void StockMovementsNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController?.NavigateToStockMovements();
            UpdateSelectedNavButton(sender as Button);
        }
        private void ReportsNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController?.NavigateToReports();
            UpdateSelectedNavButton(sender as Button);
        }

        // ***** ADD THIS CLICK HANDLER if you have a UserSettingsNavButton in XAML *****
        private void UserSettingsNavButton_Click(object sender, RoutedEventArgs e)
        {
            _mainController?.NavigateToUserSettings(); // Calls the method on the controller
            UpdateSelectedNavButton(sender as Button);
        }


        private void UpdateWindowTitle(Type? viewType)
        {
            if (viewType == null) { this.Title = "Inventory Management System - IMS Group03"; return; }
            if (viewType == typeof(Views.DashboardView)) this.Title = "IMS - Dashboard";
            else if (viewType == typeof(Views.ProductView)) this.Title = "IMS - Products";
            else if (viewType == typeof(Views.SupplierView)) this.Title = "IMS - Suppliers";
            else if (viewType == typeof(Views.PurchaseOrderView)) this.Title = "IMS - Purchase Orders";
            else if (viewType == typeof(Views.StockMovementView)) this.Title = "IMS - Stock Movements";
            else if (viewType == typeof(Views.ReportView)) this.Title = "IMS - Reports";
            else if (viewType == typeof(Views.UserSettingsView)) this.Title = "IMS - User Settings"; // Add title for new view
            else this.Title = "Inventory Management System - IMS Group03";
        }

        private void UpdateSelectedNavButton(Button? clickedButton)
        {
            if (_currentSelectedNavButton != null)
            {
                _currentSelectedNavButton.Tag = null;
                _currentSelectedNavButton.ClearValue(Button.BackgroundProperty);
                _currentSelectedNavButton.ClearValue(Button.FontWeightProperty);
            }
            _currentSelectedNavButton = clickedButton;
            if (_currentSelectedNavButton != null)
            {
                _currentSelectedNavButton.Tag = "Selected";
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}