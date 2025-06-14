// --- TEMPORARY HARDCODED LOGIN: Views/LoginWindow.xaml.cs ---
using IMS_Group03.Controllers;
using IMS_Group03.Models;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IMS_Group03
{
    public partial class LoginWindow : Window
    {
        // We are temporarily bypassing the controller.
        // private readonly LoginController _controller;

        public LoginWindow()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            // Since we are bypassing the controller, we don't need to set the DataContext for now.
            // Some bindings in the XAML might not work (like IsBusy), but the login button will.
            // _controller = App.ServiceProvider.GetRequiredService<LoginController>();
            // this.DataContext = _controller;

            UsernameTextBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // We removed async because this is no longer an async operation.
            AttemptLogin();
        }

        private void AttemptLogin()
        {
            string username = UsernameTextBox.Text;
            string password = PasswordInputBox.Password;

            // --- THIS IS THE TEMPORARY HARDCODED LOGIC ---
            if (username.Equals("admin", System.StringComparison.OrdinalIgnoreCase) && password == "password123")
            {
                // Login is successful.

                // 1. Create a fake User object to represent the logged-in user.
                var fakeUser = new User
                {
                    Id = 99, // A temporary ID
                    Username = "admin",
                    FullName = "Administrator",
                    Role = "Admin"
                };

                // 2. Get the Singleton instance of the MainController.
                var mainController = App.ServiceProvider.GetRequiredService<MainController>();

                // 3. Tell the MainController who is logged in.
                mainController.SetAuthenticatedUser(fakeUser);

                // 4. Get a new instance of the MainWindow.
                var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();

                mainWindow.Show();
                this.Close(); // Close the login window.
            }
            else
            {
                // If login fails, manually show an error message.
                MessageBox.Show("Invalid username or password. Please use 'admin' and 'password123' for this test.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptLogin();
            }
            else if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}