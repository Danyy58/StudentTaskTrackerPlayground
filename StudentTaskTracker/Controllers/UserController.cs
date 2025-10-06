using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.DTO;
using UserService.Models;
using UserService.Services;
using LoginRequest = UserService.DTO.LoginRequest;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService service) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(RegistrationRequest request)
        {
            if (request is null)
                return BadRequest();

            var result = await service.RegisterUserAsync(request);

            if (result is null)
                return BadRequest();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (request is null)
                return BadRequest();

            var token = await service.LoginAsync(request);
            if (token is null)
                return BadRequest();

            HttpContext.Response.Cookies.Append("user-cookie", token.AccessToken);
            return Ok("Logged in");
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("user-cookie", new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok("Logged Out");
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await service.DeleteAsync(userId);
            if (result is null)
                return BadRequest();

            return NoContent();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDTO request)
        {
            var result = await service.RefreshTokensAsync(request);
            if (result is null
                || result.AccessToken is null
                || result.RefreshToken is null)
                return Unauthorized();

            return Ok();
        }
    }
}
