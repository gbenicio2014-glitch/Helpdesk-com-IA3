using HelpDeskIA.Api.Data;
using HelpDeskIA.Api.Models;
using HelpDeskIA.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskIA.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase {
        private readonly AppDbContext _db;
        private readonly AuthService _auth;

        public AccountController(AppDbContext db, AuthService auth) {
            _db = db;
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto) {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { error = "Email already registered" });

            var user = new User {
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role ?? "EndUser",
                CreatedAt = DateTime.UtcNow
            };
            // store hashed password in a separate field in production; for demo, we create a simple approach
            var pwdHash = _auth.HashPassword(dto.Password);
            // use a simple approach: store hashed password in Email field? NO — create a Passwords table would be better.
            // For simplicity in this scaffold, add a temporary PasswordHash field via extension object:
            // but since User model doesn't contain it, we'll store in-memory mapping (not persistent) - WARNING in comments.
            user = await SaveUserWithPasswordHash(user, pwdHash);
            return Ok(new { user.Id, user.Email, user.Name });
        }

        // This is a scaffold helper - in production add PasswordHash property to User and persist safely.
        private async Task<User> SaveUserWithPasswordHash(User user, string pwdHash) {
            // Persist user
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            // Persist password hash in a lightweight file for the scaffold (not for production)
            var store = System.IO.Path.Combine(AppContext.BaseDirectory, $"pwdstore_{user.Id}.txt");
            await System.IO.File.WriteAllTextAsync(store, pwdHash);
            return user;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto) {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized();

            var store = System.IO.Path.Combine(AppContext.BaseDirectory, $"pwdstore_{user.Id}.txt");
            if (!System.IO.File.Exists(store)) return Unauthorized();
            var hash = await System.IO.File.ReadAllTextAsync(store);
            if (!_auth.VerifyPassword(hash, dto.Password)) return Unauthorized();

            var token = _auth.GenerateJwtToken(user.Id, user.Email, user.Role);
            return Ok(new { token });
        }
    }

    public class UserRegisterDto {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    public class UserLoginDto {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
