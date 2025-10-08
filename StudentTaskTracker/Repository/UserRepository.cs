using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserService.Data;
using UserService.Models;

namespace UserService.Repository
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task AddUser(User user)
        {
            context.User.Add(user);
            await context.SaveChangesAsync();
        }

        public async Task<User?> GetUser(Expression<Func<User, bool>> query)
        {
            var result = await context.User.FirstOrDefaultAsync(query);
            return result;
        }

        public async Task<bool> UserExists(string login)
        {
            var result = await context.User.AnyAsync(u => u.Login == login);
            return result;
        }

        public async Task DeleteUser(User user)
        {
            context.User.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task RemoveRefreshTokenAsync(int userId)
        {
            var user = context.User.Find(userId);
            user!.RefreshToken = null;
            user!.RefreshTokenExpiryTime = null;
            await context.SaveChangesAsync();
        }
    }
}
