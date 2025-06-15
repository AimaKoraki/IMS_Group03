// --- DataAccess/Repositories/IUserRepository.cs ---
using IMS_Group03.Models;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> UsernameExistsAsync(string username, int? currentUserId = null);
        Task<bool> EmailExistsAsync(string email, int? currentUserId = null);
    }
}