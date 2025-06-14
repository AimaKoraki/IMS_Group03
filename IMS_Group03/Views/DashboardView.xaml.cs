// Views/DashboardView.xaml.cs
using IMS_Group03.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IMS_Group03.Views
{
    public partial class DashboardView : UserControl
    {
        // ***** CORRECTED: Declare _controller as nullable AND readonly *****
        private readonly DashboardController? _controller;

        public DashboardView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // In design mode, _controller can remain null.
                // The d:DataContext in XAML handles basic design-time property visibility.
                return;
            }

            // Try to get controller, allowing App.ServiceProvider itself to be null (though it shouldn't be by this point)
            _controller = App.ServiceProvider?.GetService<DashboardController>();

            if (_controller == null)
            {
                // This is a critical failure if the controller is essential for the view.
                Debug.WriteLine("CRITICAL: DashboardController could not be resolved from DI in DashboardView constructor.");

                // Display an error message directly in the view's content
                this.Content = new TextBlock
                {
                    Text = "Error: The Dashboard module could not be loaded properly. Please restart the application or contact support.",
                    Margin = new Thickness(20),
                    Foreground = Brushes.OrangeRed, // Or your theme's error color
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                this.IsEnabled = false; // Disable further interaction if the view is broken
                return; // Exit constructor early; _controller remains null, but that's now allowed.
            }

            // If we reach here, _controller is successfully resolved and non-null.
            this.DataContext = _controller;

            // If you need to subscribe to PropertyChanged from the controller directly:
            // _controller.PropertyChanged += Controller_PropertyChanged; 
            // (Though direct binding via DataContext usually covers UI updates)
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // The check for DesignerProperties.GetIsInDesignMode(this) ensures this doesn't run in the designer.
            // The check for _controller != null ensures it doesn't run if DI failed in the constructor
            // and we returned early.
            if (_controller != null && !DesignerProperties.GetIsInDesignMode(this))
            {
                await _controller.LoadDashboardDataAsync();
            }
        }
    }
}