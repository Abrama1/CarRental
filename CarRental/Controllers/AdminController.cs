using CarRental.Data.DTOs;
using CarRental.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
        {
            var token = await _adminService.LoginAsync(request.Username, request.Password);
            if (token == null) return Unauthorized("Invalid username or password.");

            return Ok(new { Token = token });
        }

        [HttpPatch("approve/{rentalId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRental(int rentalId)
        {
            await _rentalService.ApproveRentalAsync(rentalId);
            return Ok("Rental approved.");
        }

        [HttpPatch("decline/{rentalId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeclineRental(int rentalId)
        {
            await _rentalService.DeclineRentalAsync(rentalId);
            return Ok("Rental declined.");
        }
    }
}
