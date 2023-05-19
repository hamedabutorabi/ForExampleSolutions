using ForExampleWebApi.Data;
using ForExampleWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForExampleWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        public UserController(UserDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<User>> Get() => await _context.Users.ToListAsync();
    }
}
