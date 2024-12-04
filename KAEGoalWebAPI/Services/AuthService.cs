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

        public async Task<string> Register(string username, string password, string role, string firstname,string lastname, string displayname, string profilepictureurl)
        {
            var existingUser = await _DbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser != null) return null;

            displayname ??= $"{firstname} {lastname}"; 

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Role = role,
                Firstname = firstname,
                Lastname = lastname,
                Displayname = displayname,
                ProfilePictureUrl = profilepictureurl
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

        public async Task<bool> UpdateProfile(int userId, UpdateProfileModel model)
        {
            var user = await _DbContext.Users.FindAsync(userId);
            if (user == null) 
                return false;

            if (!string.IsNullOrEmpty(model.Firstname))
                user.Firstname = model.Firstname;
            if (!string.IsNullOrEmpty(model.Lastname))
                user.Lastname = model.Lastname;
            if (!string.IsNullOrEmpty(model.Displayname))
                user.Displayname = model.Displayname;
            if (!string.IsNullOrEmpty(model.ProfilePictureUrl))
                user.ProfilePictureUrl = model.ProfilePictureUrl;

            _DbContext.Users.Update(user);
            await _DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<UserDetailModel> GetUserDetails(int userId)
        {
            var user = await _DbContext.Users
                .Include(u => u.Department)
                .Include(u => u.Workplace)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            return new UserDetailModel
            {
                Username = user.Username,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Displayname = user.Displayname,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Department = user.Department?.Name,
                Workplace = user.Workplace?.Name
            };
        }

        public async Task<bool> UpdateUserDetailsAsync(AdminUpdateUserDeltailsModel model)
        {
            var user = await _DbContext.Users
                .Include(u => u.Department)
                .Include(u => u.Workplace)
                .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null) return false;

            var department = await _DbContext.Departments.FindAsync(model.DepartmentId);
            var workplace = await _DbContext.Workplaces.FindAsync(model.WorkplaceId);

            if (department == null || workplace == null)
                throw new ArgumentException("Invalid DepartmentId or WorkplaceId.");

            user.DepartmentId = model.DepartmentId;
            user.WorkplaceId = model.WorkplaceId;

            _DbContext.Users.Update(user);
            await _DbContext.SaveChangesAsync();

            return true;
        }
    }
}
