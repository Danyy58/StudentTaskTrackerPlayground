using UserService.DTO;
using UserService.Models;

namespace UserService.Services
{
    public interface IUserService
    {
        public Task<User?> RegisterUserAsync(RegistrationRequest request);
        public Task<TokenDTO?> LoginAsync(LoginRequest request);
        public Task<User?> DeleteAsync(int userId);
        public Task<TokenDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request);
        public Task RemoveRefreshTokenAsync(int userId); 
    }
}
