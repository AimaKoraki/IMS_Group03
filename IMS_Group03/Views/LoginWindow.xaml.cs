// --- FULLY CORRECTED AND FINALIZED: Views/LoginWindow.xaml.cs ---
using IMS_Group03.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace IMS_Group03.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginController? _controller;

        public LoginWindow()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            _controller = App.ServiceProvider?.GetService<LoginController>();
            if (_controller == null)
            {
                MessageBox.Show("A critical error occurred: Login module could not load.", "Fatal Error");
                Application.Current.Shutdown();
                return;
            }
            this.DataContext = _controller;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null) return;

            // Securely pass the password from the PasswordBox to the controller.
            var authenticatedUser = await _controller.LoginAsync(UserPasswordBox.Password);

            // --- FIX: Check if the returned user object is not null, instead of checking a boolean. ---
            if (authenticatedUser != null)
            {
                // On successful login, get a new instance of the MainWindow
                var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

                // Close the login window
                this.Close();
            }
            // If login fails, authenticatedUser will be null, and the controller will have
            // set its ErrorMessage property, which the UI will display automatically.
        }

        // Allows the user to drag the custom window.
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}