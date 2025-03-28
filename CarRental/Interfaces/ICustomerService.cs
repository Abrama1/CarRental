using CarRental.Data.Models;
using CarRental.Data.DTOs;

namespace CarRental.Interfaces
{
    public interface ICustomerService
    {
        Task RegisterAsync(RegisterCustomerRequest dto);
        Task<bool> VerifyEmailAsync(string email, string token);
        Task<string?> LoginAsync(string email, string password);
        Task<CustomerResponse?> GetByIdAsync(int id);
        Task<Customer?> GetByEmailAsync(string email);
        Task UpdateProfileAsync(int customerId, UpdateCustomerRequest dto);
        Task DeleteCustomerAsync(int customerId);
        Task RestoreCustomerAsync(int customerId);
    }
}
