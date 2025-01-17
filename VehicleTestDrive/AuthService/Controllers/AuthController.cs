using AuthService.Model.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        public AuthController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            this._userService.AddUser(request.Username, request.Password, request.Role);
            return Created();
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            var authToken = this._userService.Authenticate(request.Username, request.Password);
            if (string.IsNullOrEmpty(authToken))
            {
                return Unauthorized();
            }
            return Ok( authToken);

        }
    }
}
