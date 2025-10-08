using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.DTO;
using UserService.Models;
using UserService.Repository.Token;

namespace UserService.Services.Token
{
    public class TokenService(IConfiguration config, ITokenRepository repo) : ITokenService
    {
        public async Task<TokenDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request)
        {
            var user = await repo.ValidateRefreshTokenAsync(request.ID, request.RefreshToken);
            if (user is null)
                return null;

            return await CreateTokenResponse(user);
        }

        public async Task<TokenDTO> CreateTokenResponse(User user)
        {
            return new TokenDTO
            {
                AccessToken = CreateToken(user),
                RefreshToken = await repo.AddRefreshTokenAsync(user)
            };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding
                .UTF8.GetBytes(config.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken
            (
                issuer: config.GetValue<string>("AppSettings:Issuer"),
                audience: config.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
