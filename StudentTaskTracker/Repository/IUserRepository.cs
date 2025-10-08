using System.Linq.Expressions;
using UserService.Models;

namespace UserService.Repository
{
    public interface IUserRepository
    {
        public Task<bool> UserExists(string login);
        public Task AddUser(User user);
        public Task<User?> GetUser(Expression<Func<User, bool>> query);
        public Task DeleteUser(User user);
        public Task RemoveRefreshTokenAsync(int userId);
    }
}
