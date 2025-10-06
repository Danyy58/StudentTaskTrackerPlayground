using System.Security.Cryptography;
using UserService.Data;
using UserService.Models;

namespace UserService.Repository.Token
{
    public class TokenRepository(AppDbContext context) : ITokenRepository
    {
        public async Task<string> AddRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var user = await context.User.FindAsync(userId);
            if (user is null
                || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
