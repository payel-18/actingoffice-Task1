using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RoleBaseApi.Data;
using RoleBaseApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoleBaseApi.Services
{
    public class AuthService
    {
        private readonly MongoContext _db;      // MongoDB database context
        private readonly JwtSettings _jwt;      // JWT configuration (Issuer, Audience, Key, etc.)

        public AuthService(MongoContext db, JwtSettings jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // This method verifies the user and generates a JWT token if credentials are correct
        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            // Find the user by email
            var user = await _db.Users.Find(u => u.Email == email).FirstOrDefaultAsync();

            // If user not found or password is wrong, return null
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            // Define the claims (information saved in the JWT token)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email), // Save user's email
                new Claim(ClaimTypes.Role, user.Role)   // Save user's role
            };

            // Convert secret key to bytes
            var keyBytes = Encoding.UTF8.GetBytes(_jwt.Key);
            var signingKey = new SymmetricSecurityKey(keyBytes);

            // Create the signing credentials using HMAC SHA-256
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiresMinutes),
                signingCredentials: creds
            );

            // Return the complete JWT token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
