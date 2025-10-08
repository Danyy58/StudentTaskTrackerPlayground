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
    public class UserController(IUserService _service) : ControllerBase
    {
        private readonly IUserService _service = _service;

        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(RegistrationRequest request)
        {
            if (request is null)
                return BadRequest();

            var result = await _service.RegisterUserAsync(request);

            if (result is null)
                return BadRequest();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (request is null)
                return BadRequest();

            var token = await _service.LoginAsync(request);
            if (token is null)
                return BadRequest();

            HttpContext.Response.Cookies.Append("user-cookie", token.AccessToken);
            return Ok("Logged in");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete("user-cookie", new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            var userId = GetUser();
            await _service.RemoveRefreshTokenAsync(userId);

            return Ok("Logged Out");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = GetUser();
            var result = await _service.DeleteAsync(userId);
            if (result is null)
                return BadRequest();

            return NoContent();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDTO request)
        {
            var result = await _service.RefreshTokensAsync(request);
            if (result is null
                || result.AccessToken is null
                || result.RefreshToken is null)
                return Unauthorized();

            return Ok();
        }

        private int GetUser()
        {
            return Convert.ToInt32(HttpContext.Request.Headers["X-User-Id"].FirstOrDefault());
        }
    }
}
