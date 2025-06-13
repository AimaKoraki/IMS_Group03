// Views/ProductView.xaml.cs
using IMS_Group03.Controllers;
using IMS_Group03.Models; // Added for 'Product' type casting from DataGrid
using Microsoft.Extensions.DependencyInjection; // For App.ServiceProvider
using System;
using System.ComponentModel; // For DesignerProperties
using System.Diagnostics;
using System.Linq; // For .Any() and .First() if needed on AvailableSuppliers
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; // For Brushes

namespace IMS_Group03.Views
{
    public partial class ProductView : UserControl
    {
        private readonly ProductController? _controller; // Nullable, will be set by DI

        public ProductView() // Constructor should NOT take parameters if view is created by XAML/NavigationService without passing them
        {
            InitializeComponent(); // This MUST be the first call

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // In design mode, the XAML d:DataContext should provide design-time data.
                // No need to instantiate a real controller here.
                return;
            }

            // Runtime: Get controller from Dependency Injection
            // Ensure App.ServiceProvider is initialized in App.xaml.cs
            if (App.ServiceProvider != null)
            {
                _controller = App.ServiceProvider.GetService<ProductController>();
            }

            if (_controller == null)
            {
                Debug.WriteLine("CRITICAL: ProductController could not be resolved from DI in ProductView. View will not function correctly.");
                // Display an error message directly in the UI if the controller is essential
                // This replaces the entire content of the UserControl.
                this.Content = new TextBlock
                {
                    Text = "Error: Product module could not be loaded.\nController is missing or not registered correctly.",
                    Margin = new Thickness(20),
                    Foreground = Brushes.OrangeRed,
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                // this.IsEnabled = false; // Optionally disable, but replacing content is more direct.
                return;
            }

            this.DataContext = _controller; // Set DataContext for XAML bindings to controller properties
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_controller != null && !DesignerProperties.GetIsInDesignMode(this))
            {
                // IsBusy should be handled by the controller and bindings.
                // If you need to show a global busy indicator from MainWindow,
                // you'd re-introduce _mainWindow logic or use a shared service.
                await _controller.LoadInitialDataAsync();
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            await _controller.LoadProductsAsync();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            _controller.PrepareNewProduct(); // This sets _controller.SelectedProductForForm

            // Check if suppliers are loaded for the ComboBox in the form
            // The placeholder "-- No Supplier --" has Id = 0
            if (_controller.AvailableSuppliers == null ||
                !_controller.AvailableSuppliers.Any() ||
                (_controller.AvailableSuppliers.Count == 1 && _controller.AvailableSuppliers.First().Id == 0))
            {
                await _controller.LoadAvailableSuppliersAsync();
            }

            // Form visibility is handled by XAML binding to _controller.SelectedProductForForm != null
            // Focus the first field after the form is made visible by the DataContext change.
            // Dispatcher ensures Focus() is called after layout updates.
            Dispatcher.BeginInvoke(new Action(() => NameTextBox?.Focus()), System.Windows.Threading.DispatcherPriority.Background);
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;

            Product? selectedProduct = ProductsDataGrid.SelectedItem as Product; // Use your Models.Product

            if (selectedProduct == null)
            {
                MessageBox.Show("Please select a product from the list to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _controller.PrepareProductForEdit(selectedProduct); // This sets _controller.SelectedProductForForm

            if (_controller.AvailableSuppliers == null ||
                !_controller.AvailableSuppliers.Any() ||
                (_controller.AvailableSuppliers.Count == 1 && _controller.AvailableSuppliers.First().Id == 0))
            {
                await _controller.LoadAvailableSuppliersAsync();
            }
            Dispatcher.BeginInvoke(new Action(() => NameTextBox?.Focus()), System.Windows.Threading.DispatcherPriority.Background);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            var (success, message) = await _controller.SaveProductAsync();
            if (success)
            {
                // HideEditFormAndClearSelection(); // Controller's ClearFormSelection is called on successful save
                MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(message, "Save Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                // _controller.ErrorMessage should be bound to ErrorMessageText in XAML
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            HideEditFormAndClearSelection();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            Product? selectedProduct = ProductsDataGrid.SelectedItem as Product; // Use your Models.Product

            if (selectedProduct == null)
            {
                MessageBox.Show("Please select a product from the list to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{selectedProduct.Name}' (SKU: {selectedProduct.Sku})?",
                                         "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var (success, message) = await _controller.DeleteProductAsync(selectedProduct.Id);
                if (success)
                {
                    MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(message, "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void HideEditFormAndClearSelection()
        {
            if (_controller == null) return;
            _controller.ClearFormSelection(); // This should cause the XAML form border to collapse
                                              // if its Visibility is bound to SelectedProductForForm (via NullToVis converter)

            // Only try to access ProductsDataGrid if InitializeComponent has successfully run
            // and the controller (and therefore DataContext) is not null.
            if (this.IsLoaded && _controller != null) // Check IsLoaded as well
            {
                ProductsDataGrid.SelectedItem = null;
            }
        }
    }
}