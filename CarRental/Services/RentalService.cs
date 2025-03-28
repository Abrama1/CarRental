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
        private readonly IEmailService _emailService;

        public RentalService(CarRentalDbContext context, IEmailService emailservice)
        {
            _context = context;
            _emailService = emailservice;
        }

        public async Task<RentalResponse?> GetByIdAsync(int rentalId)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == rentalId);

            return rental == null ? null : MapToResponse(rental);
        }

        public async Task<IEnumerable<RentalResponse>> GetAllRentalsAsync()
        {
            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .ToListAsync();

            return rentals.Select(MapToResponse);
        }

        public async Task<IEnumerable<RentalResponse>> GetRentalsForCustomerAsync(int customerId)
        {
            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();

            return rentals.Select(MapToResponse);
        }

        public async Task<RentalResponse> CreateRentalAsync(CreateRentalRequest request)
        {
            var car = await _context.Cars.FindAsync(request.CarId);
            if (car == null) throw new Exception("Car not found.");

            var rental = new Rental
            {
                CarId = request.CarId,
                CustomerId = request.CustomerId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = RentalStatus.PendingApproval,
                TotalPrice = (request.EndDate - request.StartDate).Days * car.DailyRate
            };

            _context.Rentals.Add(rental);
            car.IsAvailable = false;
            await _context.SaveChangesAsync();


            rental.Car = car;
            rental.Customer = await _context.Customers.FindAsync(request.CustomerId);

            string adminEmail = "carrentalalemailservice@gmail.com";
            string subject = "New Rental Request Received";
            string message = $@"
            Rental ID: {rental.Id}
            Car: {car.Make} {car.Model}
            Customer ID: {rental.CustomerId}
            Customer Phone: {rental.Customer.Phone}
            Start Date: {rental.StartDate:yyyy-MM-dd}
            End Date: {rental.EndDate:yyyy-MM-dd}
            Total Price: {rental.TotalPrice} ₾
            Status: {rental.Status}
            ";

            await _emailService.SendEmailAsync(adminEmail, subject, message);

            return MapToResponse(rental);
        }

        public async Task UpdateRentalAsync(int rentalId, CreateRentalRequest request)
        {
            var rental = await _context.Rentals.Include(r => r.Car).FirstOrDefaultAsync(r => r.Id == rentalId);
            if (rental == null) throw new Exception("Rental not found.");

            if (rental.CustomerId != request.CustomerId)
                throw new Exception("You are not authorized to modify this rental.");

            var today = DateTime.UtcNow.Date;

            if (rental.StartDate.Date > today)
            {
                // Future rental — allow full update
                rental.StartDate = request.StartDate;
                rental.EndDate = request.EndDate;
            }
            else if (rental.StartDate.Date <= today && rental.EndDate.Date >= today)
            {
                // Ongoing rental — allow only end date extension
                if (request.EndDate.Date < today)
                    throw new Exception("End date must be in the future.");

                rental.EndDate = request.EndDate;
            }
            else
            {
                throw new Exception("Cannot update a rental that has already ended.");
            }

            rental.TotalPrice = (rental.EndDate - rental.StartDate).Days * rental.Car.DailyRate;

            await _context.SaveChangesAsync();
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

            var car = await _context.Cars.FindAsync(rental.CarId);
            if (car != null)
            {
                switch (status)
                {
                    case RentalStatus.Approved:
                    case RentalStatus.Ongoing:
                    case RentalStatus.Reserved:
                        car.IsAvailable = false;
                        break;

                    case RentalStatus.Cancelled:
                    case RentalStatus.Completed:
                    case RentalStatus.Declined:
                        car.IsAvailable = true;
                        break;

                    case RentalStatus.PendingApproval:
                        car.IsAvailable = true;
                        break;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task ApproveRentalAsync(int rentalId)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == rentalId);
            if (rental == null) throw new Exception("Rental not found.");

            rental.Status = RentalStatus.Approved;
            rental.Car!.IsAvailable = false;

            await _context.SaveChangesAsync();

            // Send email to customer
            var subject = "Rental Approved";
            var body = $@"Hi {rental.Customer!.Name},

            Your rental request for {rental.Car.Make} {rental.Car.Model} has been approved!
            Rental period: {rental.StartDate:yyyy-MM-dd} to {rental.EndDate:yyyy-MM-dd}
            Total price: {rental.TotalPrice} ₾

            You can now proceed with pickup arrangements.";

            await _emailService.SendEmailAsync(rental.Customer.Email, subject, body);
        }


        public async Task DeclineRentalAsync(int rentalId)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == rentalId);
            if (rental == null) throw new Exception("Rental not found.");

            rental.Status = RentalStatus.Declined;
            rental.Car!.IsAvailable = true;

            await _context.SaveChangesAsync();

            // Send email to customer
            var subject = "Rental Declined";
            var body = $@"Hi {rental.Customer!.Name},

            We’re sorry to inform you that your rental request for {rental.Car.Make} {rental.Car.Model} has been declined.

            Please feel free to explore other available options.";

            await _emailService.SendEmailAsync(rental.Customer.Email, subject, body);
        }


        private static RentalResponse MapToResponse(Rental rental)
        {
            return new RentalResponse
            {
                Id = rental.Id,
                CarId = rental.CarId,
                CarMake = rental.Car?.Make ?? "",
                CarModel = rental.Car?.Model ?? "",
                CustomerId = rental.CustomerId,
                CustomerName = rental.Customer?.Name ?? "",
                CustomerPhone = rental.Customer?.Phone ?? "",
                StartDate = rental.StartDate,
                EndDate = rental.EndDate,
                TotalPrice = rental.TotalPrice,
                Status = rental.Status.ToString()
            };
        }
    }
}
