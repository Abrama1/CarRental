using CarRental.Data.DTOs;
using CarRental.Interfaces;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IRentalService _rentalService;

        public AdminController(IAdminService adminService, IRentalService rentalService)
        {
            _adminService = adminService;
            _rentalService = rentalService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
        {
            var admin = await _adminService.LoginAsync(request.Username, request.Password);
            if (admin == null) return Unauthorized("Invalid username or password.");

            return Ok($"Welcome back, {admin.Username}.");
        }

        [HttpPatch("approve/{rentalId}")]
        public async Task<IActionResult> ApproveRental(int rentalId)
        {
            await _rentalService.ApproveRentalAsync(rentalId);
            return Ok("Rental approved.");
        }

        [HttpPatch("decline/{rentalId}")]
        public async Task<IActionResult> DeclineRental(int rentalId)
        {
            await _rentalService.DeclineRentalAsync(rentalId);
            return Ok("Rental declined.");
        }

    }
}
