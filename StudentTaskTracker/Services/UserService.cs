using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.DTO;
using UserService.Models;
using UserService.Repository;
using UserService.Services.Token;

namespace UserService.Services
{
    public class UserService(ITokenService tokenService, IUserRepository repos) : IUserService
    {
        public async Task<User?> RegisterUserAsync(RegistrationRequest request)
        {
            if (await repos.UserExists(request.Login))
                return null;

            var user = new User();

            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Name = request.Name;
            user.Login = request.Login;
            user.PasswordHash = hashedPassword;
            user.Role = request.Role;

            await repos.AddUser(user);

            return user;
        }

        public async Task<TokenDTO?> LoginAsync(LoginRequest request)
        {
            var user = await repos.GetUser(u => u.Login == request.Login);
            if (user is null)
                return null;

            if (new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, request.Password) ==
                    PasswordVerificationResult.Failed)
                return null;

            return await tokenService.CreateTokenResponse(user);
        }

        public async Task<User?> DeleteAsync(int userId)
        {
            var user = await repos.GetUser(u => u.ID == userId);
            if (user is null)
                return null;

            await repos.DeleteUser(user);
            return user;
        }

        public async Task<TokenDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request)
        {
            return await tokenService.RefreshTokensAsync(request);
        }
    }
}
