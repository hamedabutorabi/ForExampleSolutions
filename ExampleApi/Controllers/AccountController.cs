using ExampleApi.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserManager<IdentityUser> _userManager;
        public AccountController(UserManager<IdentityUser> userManager)
        {

            _userManager = userManager;

        }
        // api/Account/changepassword
        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userClaims = HttpContext.User.Claims;
            var userEmail = userClaims.FirstOrDefault(c => c.Type == "Email")?.Value;
            if(userEmail == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!passwordCheckResult)
            {
                return BadRequest
                (
                    new UserManagerResponse
                    {
                        IsSuccess = false,
                        Message = "Old Password is incorrect"
                    }
                );
            }
            if(model.NewPassword != model.ConfirmPassword) 
            {
                return BadRequest
                (
                    new UserManagerResponse
                    {
                        IsSuccess = false,
                        Message = "New password and confirm new password does not match"
                    }
                );
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword,model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest
                (
                    new UserManagerResponse
                    {
                        IsSuccess = false,
                        Message = "password does not changed",
                        Errors = result.Errors.Select(e => e.Description)
                    }
                );
            }
            return Ok();
        }
    }
}
