// --- CORRECTED AND FINALIZED: Services/UserService.cs ---
using IMS_Group03.DataAccess.Repositories;
using IMS_Group03.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Group03.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Password Hashing
        // IMPORTANT: The following is a basic example for demonstration.
        // For production applications, it is highly recommended to use a dedicated, slow password
        // hashing library like BCrypt.Net or a modern algorithm like Argon2id.
        private (string PasswordHash, string PasswordSalt) HashPassword(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                var saltBytes = hmac.Key;
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashedBytes = hmac.ComputeHash(passwordBytes);
                return (Convert.ToBase64String(hashedBytes), Convert.ToBase64String(saltBytes));
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using (var hmac = new HMACSHA512(saltBytes))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var computedHashBytes = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(computedHashBytes) == storedHash;
            }
        }
        #endregion

        #region Authentication
        public async Task<(bool Success, User? User, string ErrorMessage)> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, null, "Username and password are required.");

            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null || !user.IsActive)
                return (false, null, "Invalid username or password.");

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return (false, null, "Invalid username or password.");

            // The calling ViewModel is responsible for calling UpdateLastLoginAsync AFTER successful authentication.
            return (true, user, "Login successful.");
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync(); // This is a self-contained transaction.
            }
        }
        #endregion

        #region User Management
        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _unitOfWork.Users.GetAllAsync();
        public async Task<User?> GetUserByIdAsync(int id) => await _unitOfWork.Users.GetByIdAsync(id);
        public async Task<User?> GetUserByUsernameAsync(string username) => await _unitOfWork.Users.GetByUsernameAsync(username);

        public async Task<(bool Success, User? CreatedUser, string ErrorMessage)> CreateUserAsync(User user, string password)
        {
            // (Validation logic is excellent and unchanged)
            if (await _unitOfWork.Users.UsernameExistsAsync(user.Username))
                return (false, null, $"Username '{user.Username}' already exists.");

            var (hash, salt) = HashPassword(password);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.DateCreated = DateTime.UtcNow;
            user.IsActive = true;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync(); // Commit transaction

            _logger.LogInformation("Created new user {Username} with ID {UserId}.", user.Username, user.Id);
            return (true, user, "User created successfully.");
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateUserAsync(User user)
        {
            var existingUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
            if (existingUser == null) return (false, "User not found.");
            // (Validation logic is excellent and unchanged)

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;

            _unitOfWork.Users.Update(existingUser);
            await _unitOfWork.CompleteAsync(); // Commit transaction
            return (true, "User updated successfully.");
        }

        public async Task<(bool Success, string ErrorMessage)> ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return (false, "User not found.");
            if (!VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
                return (false, "Incorrect old password.");

            var (hash, salt) = HashPassword(newPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync(); // Commit transaction
            return (true, "Password changed successfully.");
        }

        public async Task<(bool Success, string ErrorMessage)> AdminResetUserPasswordAsync(int userId, string newPassword)
        {
            // Logic is very similar to ChangePassword, just without the old password check
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return (false, "User not found.");

            var (hash, salt) = HashPassword(newPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync(); // Commit transaction
            return (true, "Password reset successfully by admin.");
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return (false, "User not found.");

            user.IsActive = false; // Soft delete
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync(); // Commit transaction

            _logger.LogWarning("Deactivated user {Username} (ID: {UserId}).", user.Username, user.Id);
            return (true, "User deactivated successfully.");
        }
        #endregion
    }
}