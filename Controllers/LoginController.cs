// --- GENERATED AND FINALIZED: Controllers/LoginController.cs ---
using IMS_Group03.Models;
using IMS_Group03.Services;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security; // For SecureString
using System.Threading.Tasks;

namespace IMS_Group03.Controllers
{
    public class LoginController : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly MainController _mainController; // To hand off the session
        private readonly ILogger<LoginController> _logger;

        #region Properties
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        // We do not store the password in a string for security.
        // The View's PasswordBox will pass it directly to the LoginAsync method.

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

        // The MainController is injected as a Singleton, so this controller gets
        // a reference to the same instance that the MainWindow uses.
        public LoginController(IUserService userService, MainController mainController, ILogger<LoginController> logger)
        {
            _userService = userService;
            _mainController = mainController;
            _logger = logger;
        }

        /// <summary>
        /// Attempts to authenticate the user and returns the authenticated user on success.
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

            try
            {
                var (success, user, message) = await _userService.AuthenticateAsync(Username, password);

                if (success && user != null)
                {
                    _logger.LogInformation("User '{Username}' authenticated successfully.", user.Username);

                    // Update the user's last login date. This is a fire-and-forget operation.
                    _ = _userService.UpdateLastLoginAsync(user.Id);

                    // Hand off the authenticated user to the main application controller.
                    _mainController.SetAuthenticatedUser(user);

                    return user; // Return the user to signal success to the View.
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
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}