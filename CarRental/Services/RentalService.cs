using CarRental.Data;
using CarRental.Data.DTOs;
using CarRental.Data.Models;
using CarRental.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    public class RentalService : IRentalService
    {
        private readonly CarRentalDbContext _context;

        public RentalService(CarRentalDbContext context)
        {
            _context = context;
        }

        public async Task<Rental?> GetByIdAsync(int rentalId)
        {
            return await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == rentalId);
        }

        public async Task<IEnumerable<Rental>> GetAllRentalsAsync()
        {
            return await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetRentalsForCustomerAsync(int customerId)
        {
            return await _context.Rentals
                .Include(r => r.Car)
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Rental> CreateRentalAsync(CreateRentalRequest request)
        {
            var rental = new Rental
            {
                CarId = request.CarId,
                CustomerId = request.CustomerId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = RentalStatus.PendingApproval
            };

            _context.Rentals.Add(rental);

            var car = await _context.Cars.FindAsync(request.CarId);
            if (car != null)
            {
                car.IsAvailable = false;
            }

            await _context.SaveChangesAsync();
            return rental;
        }

        public async Task CancelRentalAsync(int rentalId, int customerId)
        {
            var rental = await _context.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId && r.CustomerId == customerId);
            if (rental == null) return;

            _context.Rentals.Remove(rental);

            var car = await _context.Cars.FindAsync(rental.CarId);
            if (car != null)
            {
                car.IsAvailable = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CompleteRentalAsync(int rentalId)
        {
            var rental = await _context.Rentals.FindAsync(rentalId);
            if (rental == null) return;

            rental.Status = RentalStatus.Completed;

            var car = await _context.Cars.FindAsync(rental.CarId);
            if (car != null)
            {
                car.IsAvailable = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SetStatusAsync(int rentalId, RentalStatus status)
        {
            var rental = await _context.Rentals.FindAsync(rentalId);
            if (rental == null) return;

            rental.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}