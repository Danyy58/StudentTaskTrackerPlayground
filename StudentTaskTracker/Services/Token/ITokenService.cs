using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.DTO;
using UserService.Models;

namespace UserService.Services.Token
{
    public interface ITokenService
    {
        public Task<TokenDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request);
        public Task<TokenDTO> CreateTokenResponse(User user);
    }
}
