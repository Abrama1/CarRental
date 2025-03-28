using CarRental.Data.DTOs;
using CarRental.Data.Models;
using CarRental.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Create(CreateRentalRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || roleClaim == null)
                return Unauthorized("Missing user claims.");

            var userId = userIdClaim.Value;
            var role = roleClaim.Value;

            if (role != "Admin" && userId != request.CustomerId.ToString())
                return Forbid();

            var rental = await _rentalService.CreateRentalAsync(request);
            return Ok(rental);
        }

        [HttpPut("update/{rentalId}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Update(int rentalId, CreateRentalRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || roleClaim == null)
                return Unauthorized("Missing user claims.");

            var userId = userIdClaim.Value;
            var role = roleClaim.Value;

            var existingRental = await _rentalService.GetByIdAsync(rentalId);
            if (existingRental == null) return NotFound();

            if (role != "Admin" && userId != existingRental.CustomerId.ToString())
                return Forbid();

            if (existingRental.EndDate < DateTime.UtcNow)
                return BadRequest("Cannot update a rental that has already ended.");

            if (existingRental.StartDate <= request.StartDate && request.StartDate != existingRental.StartDate)
                return BadRequest($"Cannot change start date for ongoing rentals.");

            if (request.EndDate < DateTime.UtcNow)
                return BadRequest("End date must be in the future.");

            await _rentalService.UpdateRentalAsync(rentalId, request);
            return Ok("Rental updated successfully.");
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var rentals = await _rentalService.GetAllRentalsAsync();
            return Ok(rentals);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var rental = await _rentalService.GetByIdAsync(id);
            if (rental == null) return NotFound();
            return Ok(rental);
        }

        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            var role = roleClaim.Value;

            if (role != "Admin" && (userId == null || userId != customerId.ToString()))
                return Forbid("Access denied: You can only view your own rentals.");

            var rentals = await _rentalService.GetRentalsForCustomerAsync(customerId);
            return Ok(rentals);
        }

        [HttpPost("cancel/{rentalId}/customer/{customerId}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Cancel(int rentalId, int customerId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || userId != customerId.ToString())
                return Forbid("Access denied: You can only cancel your own rentals.");

            await _rentalService.CancelRentalAsync(rentalId, customerId);
            return Ok("Rental cancelled successfully.");
        }

        [HttpPatch("complete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Complete(int id)
        {
            await _rentalService.CompleteRentalAsync(id);
            return Ok("Rental completed.");
        }

        [HttpPatch("status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetStatus(int id, [FromQuery] RentalStatus status)
        {
            await _rentalService.SetStatusAsync(id, status);
            return Ok("Rental status updated.");
        }
    }
}
