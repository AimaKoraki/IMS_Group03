// Views/SupplierView.xaml.cs
using IMS_Group03.Controllers;
using IMS_Group03.Models; // For Supplier type
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; // For Brushes

namespace IMS_Group03.Views
{
    public partial class SupplierView : UserControl
    {
        private readonly SupplierController? _controller;

        public SupplierView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            if (App.ServiceProvider != null)
            {
                _controller = App.ServiceProvider.GetService<SupplierController>();
            }

            if (_controller == null)
            {
                Debug.WriteLine("CRITICAL: SupplierController could not be resolved from DI in SupplierView.");
                Content = new TextBlock { Text = "Error: Supplier module could not load.", Margin = new Thickness(20), Foreground = Brushes.Red, FontSize = 16 };
                this.IsEnabled = false;
                return;
            }
            this.DataContext = _controller;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_controller != null && !DesignerProperties.GetIsInDesignMode(this))
            {
                await _controller.LoadSuppliersAsync(); // Assumes controller has this
            }
        }

        private void ShowEditForm() // Mode (Add/Edit) is implied by _controller.SelectedSupplierForForm.Id
        {
            // Visibility and DataContext for form are handled by XAML bindings
            // to _controller.SelectedSupplierForForm
            NameTextBox?.Focus(); // Focus first field after form becomes visible
        }

        private void HideEditFormAndClearSelection()
        {
            _controller?.ClearFormSelection(); // This sets SelectedSupplierForForm to null, hiding the form
            SuppliersDataGrid.SelectedItem = null;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            await _controller.LoadSuppliersAsync();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            _controller.PrepareNewSupplier();
            ShowEditForm();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            if (SuppliersDataGrid.SelectedItem is Supplier selectedSupplier)
            {
                _controller.PrepareSupplierForEdit(selectedSupplier);
                ShowEditForm();
            }
            else
            {
                MessageBox.Show("Please select a supplier from the list to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            var (success, message) = await _controller.SaveSupplierAsync();
            if (success)
            {
                HideEditFormAndClearSelection();
                MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(message, "Save Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            Supplier selectedSupplier = SuppliersDataGrid.SelectedItem as Supplier;

            if (selectedSupplier != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete supplier '{selectedSupplier.Name}'?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var (success, message) = await _controller.DeleteSupplierAsync(selectedSupplier.Id);
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
            else
            {
                MessageBox.Show("Please select a supplier from the list to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}