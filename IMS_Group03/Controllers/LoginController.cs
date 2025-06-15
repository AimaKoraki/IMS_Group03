// --- Controllers/LoginController.cs ---
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IMS_Group03.Controllers
{
    public class LoginController : INotifyPropertyChanged
    {
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MainController _mainController;
        private readonly ILogger<LoginController> _logger;

        #region Properties (Your existing properties are perfect and unchanged)
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set { _isBusy = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            private set { _errorMessage = value; OnPropertyChanged(); }
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

       
        public LoginController(
            IServiceScopeFactory scopeFactory,
            MainController mainController,
            ILogger<LoginController> logger)
        {
            _scopeFactory = scopeFactory;
            _mainController = mainController; 
            _logger = logger;
        }

        /// <summary>
        /// Attempts to authenticate the user. The entire operation is wrapped in a scope.
        /// </summary>
        /// <param name="password">The password from the View's PasswordBox.</param>
        /// <returns>The authenticated User object if successful, otherwise null.</returns>
        public async Task<User?> LoginAsync(string password)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Username and password are required.";
                OnPropertyChanged(nameof(ErrorMessage));
                return null;
            }

            IsBusy = true;
            OnPropertyChanged(nameof(IsBusy));
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(ErrorMessage));

            // --- Create a scope for the entire login transaction. ---
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    // Resolve IUserService from the created scope.
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    // Authenticate the user using the scoped service.
                    var (success, user, message) = await userService.AuthenticateAsync(Username, password);

                    if (success && user != null)
                    {
                        _logger.LogInformation("User '{Username}' authenticated successfully.", user.Username);

                        // Update the user's last login date within the same scope/transaction.
                        await userService.UpdateLastLoginAsync(user.Id);

                        // Hand off the authenticated user to the main application controller.
                        _mainController.SetAuthenticatedUser(user);

                        return user; // Signal success to the View.
                    }
                    else
                    {
                        _logger.LogWarning("Failed login attempt for username: {Username}", Username);
                        ErrorMessage = message;
                        OnPropertyChanged(nameof(ErrorMessage));
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred during login for username: {Username}", Username);
                    ErrorMessage = "An unexpected error occurred. Please try again.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return null;
                }
                finally
                {
                    IsBusy = false;
                    OnPropertyChanged(nameof(IsBusy));
                }
            } // The scope, along with the DbContext and IUserService instance, is disposed of here.
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}