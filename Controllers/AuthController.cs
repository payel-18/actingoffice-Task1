using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RoleBaseApi.Data;
using RoleBaseApi.DTOs;
using RoleBaseApi.Models;
using RoleBaseApi.Services;
using System.Security.Claims;

namespace RoleBaseApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService; // Service for authentication
        private readonly MongoContext _db;         // MongoDB context

        public AuthController(AuthService authService, MongoContext db)
        {
            _authService = authService;
            _db = db;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            // Check if email already exists
            var exists = await _db.Users.Find(u => u.Email == request.Email).AnyAsync();
            if (exists)
                return BadRequest(new { message = "Email already exists" });

            // Create new user object
            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role.ToLower() // Save role in lowercase
            };

            // Insert user into database
            await _db.Users.InsertOneAsync(user);

            return Ok(new { message = "User registered successfully" });
        }

        // Login and return JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var token = await _authService.AuthenticateAsync(request.Email, request.Password);
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { token });
        }

        // Get profile of logged-in user
        [HttpGet("profile")]
        [Authorize]
        public IActionResult Profile()
        {
            var email = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Ok(new { email, role });
        }

        // Accessible only by admin
        [HttpGet("admin")]
        [Authorize(Roles = "admin")]
        public IActionResult AdminPage() => Ok("Welcome Admin!");

        // Accessible by admin and manager
        [HttpGet("manager")]
        [Authorize(Roles = "manager")]
        public IActionResult ManagerPage() => Ok("Welcome Manager!");
    }
}
