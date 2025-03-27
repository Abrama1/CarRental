using CarRental.Data.DTOs;
using CarRental.Data.Models;

namespace CarRental.Interfaces
{
    public interface IRentalService
    {
        Task<RentalResponse?> GetByIdAsync(int rentalId);
        Task<IEnumerable<RentalResponse>> GetAllRentalsAsync();
        Task<IEnumerable<RentalResponse>> GetRentalsForCustomerAsync(int customerId);
        Task<RentalResponse> CreateRentalAsync(CreateRentalRequest request);
        Task CancelRentalAsync(int rentalId, int customerId);
        Task CompleteRentalAsync(int rentalId);
        Task SetStatusAsync(int rentalId, RentalStatus status);
        Task ApproveRentalAsync(int rentalId);
        Task DeclineRentalAsync(int rentalId);
    }
}
