using CarRental.Data.DTOs;
using CarRental.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
        {
            var admin = await _adminService.LoginAsync(request.Username, request.Password);
            if (admin == null) return Unauthorized("Invalid username or password.");

            return Ok($"Welcome back, {admin.Username}.");
        }
    }
}
