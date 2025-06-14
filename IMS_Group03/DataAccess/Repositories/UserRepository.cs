// --- CORRECTED AND FINALIZED: DataAccess/Repositories/UserRepository.cs ---
using IMS_Group03.DataAccess;
using IMS_Group03.Models;
using Microsoft.EntityFrameworkCore;
using System; // For StringComparison
using System.Linq;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            // FIX: Use a method that translates to an index-friendly query.
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            // FIX: Use a method that translates to an index-friendly query.
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> UsernameExistsAsync(string username, int? currentUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            // FIX: Use a method that translates to an index-friendly query.
            var query = _context.Users.Where(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (currentUserId.HasValue)
            {
                query = query.Where(u => u.Id != currentUserId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? currentUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // FIX: Use a method that translates to an index-friendly query.
            var query = _context.Users.Where(u => u.Email != null && u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (currentUserId.HasValue)
            {
                query = query.Where(u => u.Id != currentUserId.Value);
            }
            return await query.AnyAsync();
        }
    }
}