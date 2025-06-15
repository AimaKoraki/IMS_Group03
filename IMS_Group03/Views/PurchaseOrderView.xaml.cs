// Views/PurchaseOrderView.xaml.cs
using IMS_Group03.Controllers; // For PurchaseOrderController and PurchaseOrderItemViewModel
using IMS_Group03.Models;
using IMS_Group03.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System; // For Exception (if you add more detailed try-catch)
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; // For Brushes (used in error display)

namespace IMS_Group03.Views
{
    public partial class PurchaseOrderView : UserControl
    {
        private readonly PurchaseOrderController? _controller;

        public PurchaseOrderView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // Design-time setup (e.g., mock controller) could go here if needed
                return;
            }

            // Runtime: Get controller from Dependency Injection
            if (App.ServiceProvider != null)
            {
                _controller = App.ServiceProvider.GetService<PurchaseOrderController>();
            }

            if (_controller == null)
            {
                Debug.WriteLine("CRITICAL: PurchaseOrderController could not be resolved from DI in PurchaseOrderView.");
                this.Content = new TextBlock
                {
                    Text = "Error: Purchase Order module could not be loaded. Controller is missing.",
                    Margin = new Thickness(20),
                    Foreground = Brushes.OrangeRed,
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                return;
            }

            this.DataContext = _controller;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_controller != null && !DesignerProperties.GetIsInDesignMode(this))
            {
                // IsBusy will be handled by controller and XAML bindings
                await _controller.LoadInitialDataAsync();
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            await _controller.LoadInitialDataAsync();
        }

        private async void NewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            _controller.PrepareNewPurchaseOrder();
            // Form visibility is handled by XAML binding to _controller.SelectedOrderForForm != null
            // Focus an initial field if the form becomes visible
            Dispatcher.BeginInvoke(new Action(() => {
                // Assuming you have a ComboBox for Supplier or a DatePicker for OrderDate
                // This part depends on the first focusable element in your new order form
                // Example: SupplierComboBoxInForm?.Focus(); // Ensure SupplierComboBoxInForm has x:Name
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private async void EditOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null || !(sender is Button button) || !(button.CommandParameter is PurchaseOrder orderToEdit))
            {
                return;
            }
            await _controller.PrepareOrderForEditAsync(orderToEdit);
            // Form visibility handled by XAML binding
            // Focus an initial field
            Dispatcher.BeginInvoke(new Action(() => {
                // Example: SupplierComboBoxInForm?.Focus();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private async void ReceiveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null || !(sender is Button button) || !(button.CommandParameter is PurchaseOrder orderToReceive))
            {
                return;
            }

            // Optional: Confirmation dialog
            var result = MessageBox.Show($"Mark PO #{orderToReceive.Id} from '{orderToReceive.Supplier?.Name}' as fully received?",
                                         "Confirm Full Receipt", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var (success, message) = await _controller.UIRecceiveFullOrderAsync(orderToReceive.Id);
                MessageBox.Show(message, success ? "Success" : "Receipt Failed", MessageBoxButton.OK,
                                success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (success)
                {
                    // Optionally refresh the main list if status change isn't auto-reflecting perfectly
                    // await _controller.LoadPurchaseOrdersAsync();
                }
            }
        }

        private async void CancelPoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null || !(sender is Button button) || !(button.CommandParameter is PurchaseOrder orderToCancel))
            {
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to cancel Purchase Order #{orderToCancel.Id}?",
                                         "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var (success, message) = await _controller.CancelPurchaseOrderAsync(orderToCancel.Id);
                MessageBox.Show(message, success ? "Success" : "Cancellation Failed", MessageBoxButton.OK,
                                success ? MessageBoxImage.Information : MessageBoxImage.Error);
                if (success)
                {
                    // Optionally refresh
                    // await _controller.LoadPurchaseOrdersAsync();
                }
            }
        }


        // --- Event Handlers for the Edit PO Form ---

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            _controller.AddItemToEditableOrder();
            // Optionally scroll the new item into view in the OrderItemsDataGrid
        }

        private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null || !(sender is Button button) || !(button.CommandParameter is PurchaseOrderItemViewModel itemVMToRemove))
            {
                return;
            }
            _controller.RemoveItemFromEditableOrder(itemVMToRemove);
        }

        private async void SavePoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;

            // Optional: Force bindings to update source if not using UpdateSourceTrigger=PropertyChanged on all inputs
            // Example: DatePickerForOrderDate.GetBindingExpression(DatePicker.SelectedDateProperty)?.UpdateSource();
            //          SupplierComboBoxInForm.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();
            //          For DataGrid items, this is trickier and usually relies on cell edit ending properly.

            var (success, message) = await _controller.SavePurchaseOrderAsync();
            if (success)
            {
                // Controller's SavePurchaseOrderAsync should call ClearFormSelection on success,
                // which hides the form via XAML binding.
                MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(message, "Save Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                // ErrorMessage property on controller is bound to ErrorMessageText in XAML
            }
        }

        private void CancelPoEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            _controller.ClearFormSelection(); // Hides form via XAML binding
        }
    }
}