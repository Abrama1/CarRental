using CarRental.Data.DTOs;
using CarRental.Data.Models;
using CarRental.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var cars = await _carService.GetAllAsync();
            return Ok(cars);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var availableCars = await _carService.GetAvailableAsync();
            return Ok(availableCars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var car = await _carService.GetByIdAsync(id);
            if (car == null) return NotFound();
            return Ok(car);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(CarCreateRequest request)
        {
            await _carService.AddAsync(request);
            return Ok("Car added successfully.");
        }


        [HttpPut("update")]
        public async Task<IActionResult> Update(CarUpdateRequest request)
        {
            await _carService.UpdateAsync(request);
            return Ok("Car updated successfully.");
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _carService.DeleteAsync(id);
            return Ok("Car deleted successfully.");
        }

        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> SetAvailability(int id, [FromQuery] bool available)
        {
            await _carService.SetAvailabilityAsync(id, available);
            return Ok("Availability updated.");
        }
    }
}
