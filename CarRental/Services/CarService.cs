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
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContext;

        public CarService(CarRentalDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContext)
        {
            _context = context;
            _env = env;
            _httpContext = httpContext;
        }

        public async Task<IEnumerable<CarResponse>> GetAllAsync()
        {
            var cars = await _context.Cars.ToListAsync();
            return cars.Select(MapToResponse);
        }

        public async Task<IEnumerable<CarResponse>> GetAvailableAsync()
        {
            var availableCars = await _context.Cars
                .Where(c => c.IsAvailable)
                .ToListAsync();
            return availableCars.Select(MapToResponse);
        }

        public async Task<CarResponse?> GetByIdAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            return car == null ? null : MapToResponse(car);
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
                IsAvailable = true
            };

            if (request.Images != null && request.Images.Count > 0)
            {
                foreach (var image in request.Images)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    car.ImageUrls.Add("/uploads/" + fileName);
                }
            }

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

            if (request.Images != null && request.Images.Count > 0)
            {
                foreach (var image in request.Images)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    existing.ImageUrls.Add("/uploads/" + fileName);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return;

            if (car.ImageUrls != null && car.ImageUrls.Count > 0)
            {
                foreach (var imageUrl in car.ImageUrls)
                {
                    var filePath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }

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

        private CarResponse MapToResponse(Car car)
        {
            var baseUrl = $"{_httpContext.HttpContext!.Request.Scheme}://{_httpContext.HttpContext.Request.Host}";
            return new CarResponse
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                DailyRate = car.DailyRate,
                Location = car.Location,
                IsAvailable = car.IsAvailable,
                LicensePlate = car.LicensePlate,
                ImageUrls = car.ImageUrls?.Select(url => $"{baseUrl}{url}").ToList() ?? new List<string>()
            };
        }
    }
}
