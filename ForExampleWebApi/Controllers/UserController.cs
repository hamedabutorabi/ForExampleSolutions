using ForExampleWebApi.Data;
using ForExampleWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ForExampleWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserDbContext _context;
        public UserController(UserDbContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }
        private User AuthenticateUser(User user)
        {
            var foundUser = _context.Users.Where(a => a.UserName == user.UserName && a.PasswordHash == user.PasswordHash).FirstOrDefault();
            if (foundUser != null)
            {
                return foundUser;
            }
            return null;
        }
        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], null,
                expires: DateTime.Now.AddDays(1),signingCredentials:credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(User user)
        {
            IActionResult response = Unauthorized();
            var user_ = AuthenticateUser(user);
            if (user_ != null)
            {
                var token = GenerateToken(user_);
                response = Ok(new { token = token });
            }
            return response;
        }
        [HttpGet]
        public async Task<IEnumerable<User>> Get() => await _context.Users.ToListAsync();

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new {id = user.Id}, user);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int id, string newPasswordHash)
        {
            var user = _context.Users.Find(id);
            if(user == null)
            {
                return NotFound();
            }
            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }

}
