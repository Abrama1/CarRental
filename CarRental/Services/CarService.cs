using CarRental.Data;
using CarRental.Data.DTOs;
using CarRental.Data.Models;
using CarRental.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    public class CarService : ICarService
    {
        private readonly CarRentalDbContext _context;

        public CarService(CarRentalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetAvailableAsync()
        {
            return await _context.Cars
                .Where(c => c.IsAvailable)
                .ToListAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task AddAsync(CarCreateRequest request)
        {
            var car = new Car
            {
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                DailyRate = request.DailyRate,
                Location = request.Location,
                LicensePlate = request.LicensePlate,
                ImageUrl = request.ImageUrl,
                IsAvailable = true
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(CarUpdateRequest request)
        {
            var existing = await _context.Cars.FindAsync(request.Id);
            if (existing == null) throw new Exception("Car not found.");

            existing.Make = request.Make;
            existing.Model = request.Model;
            existing.Year = request.Year;
            existing.DailyRate = request.DailyRate;
            existing.Location = request.Location;
            existing.IsAvailable = request.IsAvailable;
            existing.LicensePlate = request.LicensePlate;
            existing.ImageUrl = request.ImageUrl;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return;

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }

        public async Task SetAvailabilityAsync(int carId, bool isAvailable)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return;

            car.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();
        }
    }
}
