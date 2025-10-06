using UserService.Models;

namespace UserService.Repository.Token
{
    public interface ITokenRepository
    {
        public Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken);
        public Task<string> AddRefreshTokenAsync(User user);
    }
}
