// Services/IUserService.cs
using IMS_Group03.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMS_Group03.DataAccess.Repositories;

namespace IMS_Group03.Services
{
    public interface IUserService
    {
        // Existing User Management Methods
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<(bool Success, User? CreatedUser, string ErrorMessage)> CreateUserAsync(User user, string password);
        Task<(bool Success, string ErrorMessage)> UpdateUserAsync(User user);
        Task<(bool Success, string ErrorMessage)> ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword);
        Task<(bool Success, string ErrorMessage)> AdminResetUserPasswordAsync(int userId, string newPassword);
        Task<(bool Success, string ErrorMessage)> DeleteUserAsync(int id); // Likely soft delete
        Task<User?> GetUserByUsernameAsync(string username);

        // ADDED Authentication Related Methods
        Task<(bool Success, User? User, string ErrorMessage)> AuthenticateAsync(string username, string password);
        Task UpdateLastLoginAsync(int userId);
    }
}