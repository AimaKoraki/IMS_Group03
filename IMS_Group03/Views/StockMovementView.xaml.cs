// Views/StockMovementView.xaml.cs
using IMS_Group03.Controllers;
using IMS_Group03.Models; // For Product in PrepareNewAdjustment
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IMS_Group03.Views
{
    public partial class StockMovementView : UserControl
    {
        private readonly StockMovementController _controller;

        public StockMovementView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            _controller = App.ServiceProvider.GetService<StockMovementController>();
            if (_controller == null)
            {
                Debug.WriteLine("CRITICAL: StockMovementController could not be resolved.");
                Content = new TextBlock { Text = "Error: Stock Movement module could not load.", Margin = new Thickness(20), Foreground = Brushes.Red, FontSize = 16 };
                return;
            }
            this.DataContext = _controller;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_controller != null && !DesignerProperties.GetIsInDesignMode(this))
            {
                // Controller's SelectedProductFilter property setter should trigger movement load
                await _controller.LoadInitialDataAsync(); // Loads AvailableProducts
                // Optionally, select a default product or leave it for user to select
            }
        }

        private async void PerformAdjustmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;

            // Trigger binding updates manually if not using UpdateSourceTrigger=PropertyChanged on TextBoxes
            // AdjustmentProductComboBox.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();
            // AdjustmentQuantityTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            // AdjustmentReasonTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            // AdjustmentPerformedByTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();


            var (success, message) = await _controller.PerformStockAdjustmentAsync();
            if (success)
            {
                MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Form is cleared by controller
            }
            else
            {
                MessageBox.Show(message, "Adjustment Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearAdjustmentFormButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            _controller.ClearAdjustmentForm();
            // Optionally, also reset the ComboBox for product selection in the adjustment form if it's separate from the filter
            // AdjustmentProductComboBox.SelectedItem = null; // Or bind SelectedItem to a separate controller property
        }

        // Note: The filter ComboBox (ProductFilterComboBox) relies on the TwoWay binding
        // to _controller.SelectedProductFilter. The setter for SelectedProductFilter
        // in StockMovementController should then call LoadMovementsForProductAsync(value.Id).
    }
}