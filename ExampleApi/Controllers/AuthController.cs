using ExampleApi.Services;
using ExampleApi.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IMailService _mailService;
        public AuthController(IUserService userService, IMailService mailService)
        {
            _userService = userService;
            _mailService = mailService;
        }
        // /api/auth/register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);
                if (result.IsSuccess)
                {
                    await _mailService.SendEmailAsync(model.Email, "Welcome to Example Zo", $"Dear User, your registration has completed. You can login to website Now (Link)");
                    return Ok(result); // 200
                }
                return BadRequest(result);
            }
            return BadRequest("some properties are not valid"); // status code 400

        }

        // /api/auth/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("some properties are not valid!");
        }
    }
}
