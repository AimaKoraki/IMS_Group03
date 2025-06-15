// ---DataAccess/Repositories/UserRepository.cs ---
using IMS_Group03.DataAccess;
using IMS_Group03.Models;
using Microsoft.EntityFrameworkCore;
// using System; // No longer needed for StringComparison

namespace IMS_Group03.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            // Convert both sides to uppercase for a case-insensitive, translatable query.
            var upperUsername = username.ToUpper();
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToUpper() == upperUsername);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            // Convert both sides to uppercase for a case-insensitive, translatable query.
            var upperEmail = email.ToUpper();
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToUpper() == upperEmail);
        }

        public async Task<bool> UsernameExistsAsync(string username, int? currentUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            //  Convert both sides to uppercase for a case-insensitive, translatable query.
            var upperUsername = username.ToUpper();
            var query = _context.Users.Where(u => u.Username.ToUpper() == upperUsername);

            if (currentUserId.HasValue)
            {
                query = query.Where(u => u.Id != currentUserId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? currentUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            // Convert both sides to uppercase for a case-insensitive, translatable query.
            var upperEmail = email.ToUpper();
            var query = _context.Users.Where(u => u.Email != null && u.Email.ToUpper() == upperEmail);

            if (currentUserId.HasValue)
            {
                query = query.Where(u => u.Id != currentUserId.Value);
            }
            return await query.AnyAsync();
        }
    }
}