using ExampleApi.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExampleApi.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);
        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);
    }
    public class UserService : IUserService
    {
        private IConfiguration _configuration;
        TokenSettings tokenSettings;
        private UserManager<IdentityUser> _userManager;
        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration, TokenSettings tokenSettings)
        {
            _configuration = configuration;
            _userManager = userManager;
            this.tokenSettings = tokenSettings;
        }
        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if(model == null)
            {
                throw new NullReferenceException("Register model is null");
            }
            var identityUser = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };
            if(model.Password != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "password and confirm password does not match!",
                };
            }    
            var result = await _userManager.CreateAsync(identityUser,model.Password);
            if(result.Succeeded)
            {
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "User Created Successfully!"
                };
            }
            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "user did not create",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "There is no user with this email address"
                };
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if(!result)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Password is incorrect"
                };
            }
            var claims = new[]
            {
                new Claim("Email",model.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret));

            var token = new JwtSecurityToken(
                issuer: tokenSettings.Issuer,
                audience: tokenSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials (key,SecurityAlgorithms.HmacSha256));
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new UserManagerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }
    }
}
