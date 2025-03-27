using CarRental.Data.Models;

namespace CarRental.Interfaces
{
    public interface IAdminService
    {
        Task<string?> LoginAsync(string username, string password);
    }
}
