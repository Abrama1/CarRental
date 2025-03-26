using CarRental.Data.DTOs;
using CarRental.Data.Models;

namespace CarRental.Interfaces
{
    public interface IRentalService
    {
        Task<Rental?> GetByIdAsync(int rentalId);
        Task<IEnumerable<Rental>> GetAllRentalsAsync();
        Task<IEnumerable<Rental>> GetRentalsForCustomerAsync(int customerId);
        Task<Rental> CreateRentalAsync(CreateRentalRequest request);
        Task CancelRentalAsync(int rentalId, int customerId);
        Task CompleteRentalAsync(int rentalId);
        Task SetStatusAsync(int rentalId, RentalStatus status);
    }
}