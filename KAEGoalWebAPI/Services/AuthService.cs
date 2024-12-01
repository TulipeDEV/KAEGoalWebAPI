using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KAEGoalWebAPI.Data;
using KAEGoalWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace KAEGoalWebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _DbContext;
        private readonly IConfiguration _Configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _DbContext = context;
            _Configuration = configuration;
        }

        public async Task<string> Register(string username, string password, string role)
        {
            var existingUser = await _DbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser != null) return null;

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Role = role
            };

            _DbContext.Users.Add(user);
            await _DbContext.SaveChangesAsync();
            return GenerateJwtToken(user);
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !VerifyPassword(password, user.PasswordHash)) return null;

            var refreshToken = GenerateRefreshToken(user.Id);
            await _DbContext.RefreshTokens.AddAsync(refreshToken);
            await _DbContext.SaveChangesAsync();

            return GenerateJwtToken(user);
        }

        public async Task<string> RefreshToken(string refreshToken)
        {
            var token = await _DbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (token == null || token.ExpiryDate < DateTime.Now) return null;

            var user = await _DbContext.Users.FindAsync(token.UserId);
            return user == null ? null : GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _Configuration["Jwt:Issuer"],
                audience: _Configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.Now.AddDays(7),
                UserId = userId
            };

            return refreshToken;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }

        private bool VerifyPassword(string password, string storedPasswordHash)
        {
            return storedPasswordHash == HashPassword(password);
        }
    }
}
