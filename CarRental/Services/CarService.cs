using CarRental.Data;
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

        public async Task AddAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            var existing = await _context.Cars.FindAsync(car.Id);
            if (existing == null) return;

            existing.Make = car.Make;
            existing.Model = car.Model;
            existing.Year = car.Year;
            existing.DailyRate = car.DailyRate;
            existing.Location = car.Location;
            existing.IsAvailable = car.IsAvailable;
            existing.LicensePlate = car.LicensePlate;
            existing.ImageUrl = car.ImageUrl;

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
