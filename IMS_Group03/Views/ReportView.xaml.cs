// Views/ReportView.xaml.cs
using IMS_Group03.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace IMS_Group03.Views
{
    public partial class ReportView : UserControl
    {
        private ReportController _controller;
        // No direct _mainWindow reference needed here unless pushing status globally

        public ReportView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            _controller = App.ServiceProvider.GetService<ReportController>();
            if (_controller == null)
            {
                MessageBox.Show("ReportController could not be resolved. Reports may not function.", "Controller Error");
                this.IsEnabled = false;
                return;
            }
            this.DataContext = _controller;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_controller != null && !DesignerProperties.GetIsInDesignMode(this))
            {
                await _controller.LoadInitialDataAsync(); // Load product list for filter, etc.
            }
        }

        private async void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller != null)
            {
                await _controller.GenerateReportAsync();
            }
        }
    }
}