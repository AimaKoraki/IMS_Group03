// --- FULLY CORRECTED AND FINALIZED: Views/ProductView.xaml.cs ---
using IMS_Group03.Controllers;
using IMS_Group03.Models;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace IMS_Group03.Views
{
    public partial class ProductView : UserControl
    {
        private readonly ProductController? _controller;

        public ProductView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            _controller = App.ServiceProvider?.GetService<ProductController>();
            if (_controller == null)
            {
                Content = new TextBlock { Text = "Error: Product module could not load." };
                return;
            }
            this.DataContext = _controller;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_controller != null) await _controller.LoadInitialDataAsync();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller != null)
            {

                await _controller.LoadInitialDataAsync();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // This now correctly calls the method in the controller.
            _controller?.PrepareNewProduct();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller?.SelectedProductGridItem != null)
            {
                _controller.PrepareProductForEdit(_controller.SelectedProductGridItem);
            }
            else
            {
                MessageBox.Show("Please select a product from the list to edit.", "No Selection");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller?.SelectedProductGridItem != null)
            {
                if (MessageBox.Show($"Delete '{_controller.SelectedProductGridItem.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    var (success, message) = await _controller.DeleteProductAsync(_controller.SelectedProductGridItem.Id);
                    if (!success) MessageBox.Show(message, "Delete Failed");
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "No Selection");
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;
            var (success, message) = await _controller.SaveProductAsync();
            if (!success) MessageBox.Show(message, "Save Failed");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _controller?.ClearFormSelection();
        }
    }
}