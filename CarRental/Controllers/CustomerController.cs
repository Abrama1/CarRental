using CarRental.Data.DTOs;
using CarRental.Data.Models;
using CarRental.Interfaces;
using Microsoft.AspNetCore.Mvc;

// Alias to avoid conflict with Microsoft.AspNetCore.Identity.Data.LoginRequest
using LoginDto = CarRental.Data.DTOs.LoginRequest;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IEmailService _emailService;

        public CustomerController(ICustomerService customerService, IEmailService emailService)
        {
            _customerService = customerService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCustomerRequest request)
        {
            await _customerService.RegisterAsync(request);
            return Ok("Registration successful. Please check your email to verify your account.");
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var result = await _customerService.VerifyEmailAsync(email, token);
            if (!result) return BadRequest("Invalid or expired token.");
            return Ok("Email verified successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var customer = await _customerService.LoginAsync(request.Email, request.Password);
            if (customer == null) return Unauthorized("Invalid credentials or account not verified.");
            return Ok(customer);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCustomerRequest request)
        {
            await _customerService.UpdateProfileAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}
