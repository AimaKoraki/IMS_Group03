    // Views/UserSettingsView.xaml.cs
    using IMS_Group03.Controllers;
    using IMS_Group03.Models;     // For User model
    using Microsoft.Extensions.DependencyInjection;
    using System;                   // For Action
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    namespace IMS_Group03.Views
    {
        public partial class UserSettingsView : UserControl
        {
            private readonly UserSettingsController? _controller;

            public UserSettingsView()
            {
                InitializeComponent(); // This MUST be the first call.

                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    return;
                }

                if (App.ServiceProvider != null)
                {
                    _controller = App.ServiceProvider.GetService<UserSettingsController>();
                }

                if (_controller == null)
                {
                    Debug.WriteLine("CRITICAL: UserSettingsController could not be resolved from DI in UserSettingsView.");
                    this.Content = new TextBlock
                    {
                        Text = "Error: User Settings module could not load.\nController is missing or not registered correctly.",
                        Margin = new Thickness(20),
                        Foreground = Brushes.OrangeRed,
                        FontSize = 16,
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    return;
                }
                this.DataContext = _controller; // Set DataContext for XAML bindings
            }

            private async void UserControl_Loaded(object sender, RoutedEventArgs e)
            {
                if (_controller != null && !DesignerProperties.GetIsInDesignMode(this))
                {
                    await _controller.LoadUsersAsync();
                }
            }

            // --- TOOLBAR BUTTON EVENT HANDLERS ---
            private void RefreshButton_Click(object sender, RoutedEventArgs e)
            {
                _controller?.LoadUsersAsync();
            }

            private void AddNewButton_Click(object sender, RoutedEventArgs e)
            {
                if (_controller == null) return;
                _controller.PrepareNewUser(); // Controller sets SelectedUserForForm to null (or new blank user for form fields)
                                              // This will show the form (if bound to SelectedUserForForm != null)
                                              // and clear controller's input properties.
                UserPasswordBox.Clear();
                UserConfirmPasswordBox.Clear();
                // Focus the first field after the form is made visible.
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Ensure UsernameTextBox is found by x:Name from XAML
                    var usernameTb = this.FindName("UsernameTextBox") as TextBox;
                    usernameTb?.Focus();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }

            private void EditSelectedButton_Click(object sender, RoutedEventArgs e)
            {
                if (_controller == null) return;

                // SelectedUserForGrid is bound to DataGrid's SelectedItem
                if (_controller.SelectedUserForGrid != null)
                {
                    _controller.SelectUserForEdit(_controller.SelectedUserForGrid); // Controller populates its form input fields
                                                                                    // and sets SelectedUserForForm, making form visible.
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var usernameTb = this.FindName("UsernameTextBox") as TextBox;
                        usernameTb?.Focus();
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
                else
                {
                    MessageBox.Show("Please select a user from the list to edit.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            private async void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
            {
                if (_controller == null || _controller.SelectedUserForGrid == null)
                {
                    MessageBox.Show("Please select a user from the list to delete.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                User userToDelete = _controller.SelectedUserForGrid;
                var result = MessageBox.Show($"Are you sure you want to deactivate user '{userToDelete.Username}'?",
                                                "Confirm Deactivation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var (success, message) = await _controller.DeleteUserAsync(userToDelete.Id);
                    MessageBox.Show(message, success ? "Success" : "Error", MessageBoxButton.OK,
                                    success ? MessageBoxImage.Information : MessageBoxImage.Error);
                    // The controller's DeleteUserAsync should handle refreshing the list
                    // and clearing SelectedUserForGrid/SelectedUserForForm if the deleted user was selected.
                }
            }

            // --- FORM BUTTON EVENT HANDLERS ---
            private async void SaveUserButton_Click(object sender, RoutedEventArgs e)
            {
                if (_controller == null) return;

                _controller.PasswordInput = UserPasswordBox.Password;
                _controller.ConfirmPasswordInput = UserConfirmPasswordBox.Password;

                var (success, message) = await _controller.SaveUserAsync();
                MessageBox.Show(message, success ? "Success" : "Operation Status", MessageBoxButton.OK,
                                success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (success)
                {
                    UserPasswordBox.Clear();
                    UserConfirmPasswordBox.Clear();
                    // The controller's SaveUserAsync calls PrepareNewUser() on success,
                    // which should set _controller.SelectedUserForForm to null, hiding the form via XAML binding.
                }
            }

            private void ClearFormButton_Click(object sender, RoutedEventArgs e)
            {
                if (_controller == null) return;
                _controller.PrepareNewUser(); // Controller clears its form input properties and SelectedUserForForm
                UserPasswordBox.Clear();
                UserConfirmPasswordBox.Clear();
            }
        }
    }