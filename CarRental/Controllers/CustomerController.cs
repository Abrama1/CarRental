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
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterCustomerRequest request)
        {
            await _customerService.RegisterAsync(request);
            return Ok("Registration successful. Please check your email to verify your account.");
        }

        [HttpGet("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var result = await _customerService.VerifyEmailAsync(email, token);
            if (!result) return BadRequest("Invalid or expired token.");
            return Ok("Email verified successfully.");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _customerService.LoginAsync(request.Email, request.Password);
            if (token == null) return Unauthorized("Invalid credentials or account not verified.");
            return Ok(new { Token = token });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (userId != id) return Forbid();

            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(int id, UpdateCustomerRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (userId != id) return Forbid();

            await _customerService.UpdateProfileAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (userId != id) return Forbid();

            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}
