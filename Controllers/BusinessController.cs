using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RoleBaseApi.Models;
using RoleBaseApi.Data;
using RoleBaseApi.DTOs;
using System.Security.Claims;

namespace RoleBaseApi.Controllers
{
    [ApiController]
    [Route("api/businesses")]
    [Authorize]
    public class BusinessController: ControllerBase
    {
        private readonly MongoContext _db;

        public BusinessController(MongoContext db)
        {
            _db = db;
        }

        // Add new business
        [HttpPost("add")]
        public async Task<IActionResult> AddBusiness([FromBody] AddBusinessDto businessDto)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new { message = "User not found in token." });

            // Business object to insert
            var business = new Business
            {
                CreatedBy = userEmail,
                Type = businessDto.Type,
                Name = businessDto.Name
            };

            await _db.Businesses.InsertOneAsync(business);
            return Ok(new { message = "Business added successfully", business });
        }

        // Get businesses
        [HttpGet("list")]
        public async Task<IActionResult> GetBusinesses()
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var userEmail = User.Identity?.Name;

            List<Business> businesses;

            if (userRole == "admin")
            {
                // Admin can see all businesses
                businesses = await _db.Businesses.Find(_ => true).ToListAsync();
            }
            else
            {
                // Others can only see their businesses
                businesses = await _db.Businesses.Find(b => b.CreatedBy == userEmail).ToListAsync();
            }

            return Ok(businesses);
        }
    }
}
