using CarRental.Data.Models;

namespace CarRental.Interfaces
{
    public interface IAdminService
    {
        Task<AdminUser?> LoginAsync(string username, string password);
    }
}
