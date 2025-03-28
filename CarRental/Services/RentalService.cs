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
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ccc; border-radius: 10px; max-width: 600px; margin: auto; background-color: #fdfdfd;'>
                <h2 style='color: #2c3e50; margin-top: 0;'>📄 New Rental Request</h2>

                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 8px;'><strong>Rental ID:</strong></td>
                        <td style='padding: 8px;'>{rental.Id}</td>
                    </tr>
                    <tr style='background-color: #f9f9f9;'>
                        <td style='padding: 8px;'><strong>Car:</strong></td>
                        <td style='padding: 8px;'>{car.Make} {car.Model}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px;'><strong>Customer ID:</strong></td>
                        <td style='padding: 8px;'>{rental.CustomerId}</td>
                    </tr>
                    <tr style='background-color: #f9f9f9;'>
                        <td style='padding: 8px;'><strong>Phone:</strong></td>
                        <td style='padding: 8px;'>{rental.Customer.Phone}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px;'><strong>Start Date:</strong></td>
                        <td style='padding: 8px;'>{rental.StartDate:yyyy-MM-dd}</td>
                    </tr>
                    <tr style='background-color: #f9f9f9;'>
                        <td style='padding: 8px;'><strong>End Date:</strong></td>
                        <td style='padding: 8px;'>{rental.EndDate:yyyy-MM-dd}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px;'><strong>Total Price:</strong></td>
                        <td style='padding: 8px;'>{rental.TotalPrice} ₾</td>
                    </tr>
                    <tr style='background-color: #f9f9f9;'>
                        <td style='padding: 8px;'><strong>Status:</strong></td>
                        <td style='padding: 8px; color: #d35400;'><strong>{rental.Status}</strong></td>
                    </tr>
                </table>

                <hr style='margin: 30px 0; border: none; border-top: 1px solid #ccc;' />

                <footer style='font-size: 0.9em; color: #777;'>
                    <p>CarRental Inc. | <a href='mailto:contact@carrental.com' style='color: #3498db;'>contact@carrental.com</a></p>
                    <p>
                        <a href='https://facebook.com' style='margin-right: 10px; color: #3b5998;'>Facebook</a>
                        <a href='https://twitter.com' style='margin-right: 10px; color: #1da1f2;'>Twitter</a>
                        <a href='https://instagram.com' style='color: #e1306c;'>Instagram</a>
                    </p>
                </footer>
            </div>";



            await _emailService.SendEmailAsync(adminEmail, subject, message);

            return MapToResponse(rental);
        }

        public async Task UpdateRentalAsync(int rentalId, CreateRentalRequest request)
        {
            var rental = await _context.Rentals.Include(r => r.Car).Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == rentalId);
            if (rental == null) throw new Exception("Rental not found.");

            if (rental.CustomerId != request.CustomerId)
                throw new Exception("You are not authorized to modify this rental.");

            var today = DateTime.UtcNow.Date;

            if (rental.StartDate.Date > today)
            {
                rental.StartDate = request.StartDate;
                rental.EndDate = request.EndDate;
            }
            else if (rental.StartDate.Date <= today && rental.EndDate.Date >= today)
            {
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

            // Email admin about the update
            string adminEmail = "carrentalalemailservice@gmail.com";
            string subject = "Rental Updated by Customer";
            string message = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ccc; border-radius: 10px; max-width: 600px; margin: auto; background-color: #fdfdfd;'>
                <h2 style='color: #2980b9; margin-top: 0;'>🛠️ Rental Updated</h2>

                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 8px;'><strong>Rental ID:</strong></td>
                        <td style='padding: 8px;'>{rental.Id}</td>
                    </tr>
                    <tr style='background-color: #f9f9f9;'>
                        <td style='padding: 8px;'><strong>Car:</strong></td>
                        <td style='padding: 8px;'>{rental.Car.Make} {rental.Car.Model}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px;'><strong>Customer:</strong></td>
                        <td style='padding: 8px;'>{rental.Customer.Name} ({rental.Customer.Email})</td>
                    </tr>
                    <tr style='background-color: #f9f9f9;'>
                        <td style='padding: 8px;'><strong>New Start Date:</strong></td>
                        <td style='padding: 8px;'>{rental.StartDate:yyyy-MM-dd}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px;'><strong>New End Date:</strong></td>
                        <td style='padding: 8px;'>{rental.EndDate:yyyy-MM-dd}</td>
                    </tr>
                    <tr style='background-color: #f9f9f9;'>
                        <td style='padding: 8px;'><strong>Updated Total Price:</strong></td>
                        <td style='padding: 8px;'>{rental.TotalPrice} ₾</td>
                    </tr>
                </table>

                <hr style='margin: 30px 0; border: none; border-top: 1px solid #ccc;' />

                <footer style='font-size: 0.9em; color: #777;'>
                    <p>CarRental Inc. | <a href='mailto:contact@carrental.com' style='color: #3498db;'>contact@carrental.com</a></p>
                    <p>
                        <a href='https://facebook.com' style='margin-right: 10px; color: #3b5998;'>Facebook</a>
                        <a href='https://twitter.com' style='margin-right: 10px; color: #1da1f2;'>Twitter</a>
                        <a href='https://instagram.com' style='color: #e1306c;'>Instagram</a>
                    </p>
                </footer>
            </div>";



            await _emailService.SendEmailAsync(adminEmail, subject, message);
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
            var body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #4CAF50; border-radius: 10px; max-width: 600px; margin: auto; background-color: #f6fffa;'>
                <h2 style='color: #2e7d32;'>✅ Rental Approved</h2>
                <p>Hi {rental.Customer!.Name},</p>
                <p>Your rental request for <strong>{rental.Car.Make} {rental.Car.Model}</strong> has been <strong>approved</strong>! 🎉</p>

                <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                    <tr>
                        <td style='padding: 8px;'><strong>Rental Period:</strong></td>
                        <td style='padding: 8px;'>{rental.StartDate:yyyy-MM-dd} to {rental.EndDate:yyyy-MM-dd}</td>
                    </tr>
                    <tr style='background-color: #f0f0f0;'>
                        <td style='padding: 8px;'><strong>Total Price:</strong></td>
                        <td style='padding: 8px;'>{rental.TotalPrice} ₾</td>
                    </tr>
                </table>

                <p style='margin-top: 20px;'>You can now proceed with the pickup arrangements.</p>

                <hr style='margin: 30px 0; border: none; border-top: 1px solid #ccc;' />
                <footer style='font-size: 0.9em; color: #777;'>
                    <p>CarRental Inc. | <a href='mailto:contact@carrental.com' style='color: #3498db;'>contact@carrental.com</a></p>
                </footer>
            </div>";

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
            var body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #e74c3c; border-radius: 10px; max-width: 600px; margin: auto; background-color: #fff6f6;'>
                <h2 style='color: #c0392b;'>❌ Rental Declined</h2>
                <p>Hi {rental.Customer!.Name},</p>
                <p>We’re sorry to inform you that your rental request for <strong>{rental.Car.Make} {rental.Car.Model}</strong> has been <strong>declined</strong>.</p>
                <p>Please feel free to explore other available vehicles on our platform — we’re here to help you find the right fit.</p>

                <hr style='margin: 30px 0; border: none; border-top: 1px solid #ccc;' />
                <footer style='font-size: 0.9em; color: #777;'>
                    <p>CarRental Inc. | <a href='mailto:contact@carrental.com' style='color: #3498db;'>contact@carrental.com</a></p>
                </footer>
            </div>";

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
