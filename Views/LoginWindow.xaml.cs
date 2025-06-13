// LoginWindow.xaml.cs
using IMS_Group03.Models; // If you create a User model later
// using IMS_Group03.Services; // If you create an IAuthService
using Microsoft.Extensions.DependencyInjection; // For App.ServiceProvider
using System;
using System.Windows;
using System.Windows.Input; // For KeyEventArgs

namespace IMS_Group03
{
    public partial class LoginWindow : Window
    {
        // In a real application, inject an IAuthService (or similar)
        // private readonly IAuthService _authService;

        // For this example, using hardcoded credentials.
        // NEVER do this in a production application.
        private const string CorrectUsername = "admin";
        private const string CorrectPassword = "password123";

        public LoginWindow(/* IAuthService authService */)
        {
            InitializeComponent();
            // _authService = authService; // If using DI for auth service

            UsernameTextBox.Focus(); // Set initial focus
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await AttemptLogin();
        }

        private async System.Threading.Tasks.Task AttemptLogin()
        {
            string username = UsernameTextBox.Text;
            string password = PasswordInputBox.Password;

            ErrorMessageText.Text = ""; // Clear previous error messages

            if (string.IsNullOrWhiteSpace(username))
            {
                ErrorMessageText.Text = "Username is required.";
                UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessageText.Text = "Password is required.";
                PasswordInputBox.Focus();
                return;
            }

            // Simulate network delay and processing
            LoginBtn.IsEnabled = false; // Disable button during processing
            // Show some kind of busy indicator if you have one

            // In a real app, this would be an async call to your auth service:
            // bool isAuthenticated = await _authService.LoginAsync(username, password);
            bool isAuthenticated = AuthenticateUserLocally(username, password); // Local hardcoded check

            await System.Threading.Tasks.Task.Delay(300); // Simulate processing time

            LoginBtn.IsEnabled = true; // Re-enable button

            if (isAuthenticated)
            {
                // Successful login
                // Resolve MainWindow from DI to ensure its dependencies are also resolved
                var mainWindow = App.ServiceProvider?.GetService<MainWindow>();

                if (mainWindow != null)
                {
                    mainWindow.Show();
                    this.Close(); // Close the login window
                }
                else
                {
                    ErrorMessageText.Text = "Error: Could not load the main application window. Please contact support.";
                    // Log this critical error
                    System.Diagnostics.Debug.WriteLine("CRITICAL ERROR: MainWindow could not be resolved from ServiceProvider after login.");
                }
            }
            else
            {
                // Failed login
                ErrorMessageText.Text = "Invalid username or password. Please try again.";
                PasswordInputBox.Clear(); // Clear password field
                PasswordInputBox.Focus();
            }
        }

        private bool AuthenticateUserLocally(string username, string password)
        {
            // THIS IS FOR DEMONSTRATION ONLY.
            // Replace with a secure call to an authentication service that checks credentials
            // against a database (hashed passwords + salt) or an identity provider.
            return username.Equals(CorrectUsername, StringComparison.OrdinalIgnoreCase) &&
                   password.Equals(CorrectPassword); // Password check should be case-sensitive
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Exit the entire application
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Check which control has focus to avoid triggering login from password box before it updates
                if (PasswordInputBox.IsFocused || UsernameTextBox.IsFocused || LoginBtn.IsFocused)
                {
                    await AttemptLogin();
                }
            }
            else if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
        }
    }
}