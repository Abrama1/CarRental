using CarRental.Data.DTOs;
using CarRental.Data.Models;
using CarRental.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var rental = await _rentalService.CreateRentalAsync(request);
            return Ok(rental);
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

            if (userId == null || userId != customerId.ToString())
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
