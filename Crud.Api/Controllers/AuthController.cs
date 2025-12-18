using Crud.Api.Data;
using Crud.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Crud.Api.DTOs.AuthDTOs;

namespace Crud.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // REGISTER A NEW USER
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            if (await _db.Users.AnyAsync(u => u.UserName == dto.UserName))
                return BadRequest("Username already exists.");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                UserName = dto.UserName,
                EmailVerificationToken = Guid.NewGuid().ToString()
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Send email with verification link
            // {BaseUrl}/api/auth/verify-email?token=XYZ

            return Ok("User registered successfully. Please verify your email.");
        }

        // VERIFY EMAIL
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
            if (user == null)
                return BadRequest("Invalid token.");
            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            await _db.SaveChangesAsync();
            return Ok("Email verified successfully.");
        }

        // LOGIN USER
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            if (!user.EmailVerified)
                return Unauthorized("Email not verified.");

            // Generate JWT token
            var token = GenerateJwt(user);
            return Ok(new { Token = "JWT_TOKEN_HERE" });
        }

        // FORGOT PASSWORD
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Ok("If email exists, you will receive a reset password mail in your inbox.");

            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);

            await _db.SaveChangesAsync();

            // TODO: send email with reset link

            return Ok("Password reset link sent");
        }

        // RESET PASSWORD
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.ResetToken == dto.Token &&
                u.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
                return BadRequest("Invalid or expired token");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _db.SaveChangesAsync();
            return Ok("Password reset successful");
        }
        //JWT Generation
        private string GenerateJwt(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var token = new JwtSecurityToken(

                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
