using CarRental.Data.DTOs;
using CarRental.Data.Models;

namespace CarRental.Interfaces
{
    public interface ICarService
    {
        Task<IEnumerable<CarResponse>> GetAllAsync();
        Task<IEnumerable<CarResponse>> GetAvailableAsync();
        Task<CarResponse?> GetByIdAsync(int id);
        Task AddAsync(CarCreateRequest request);
        Task UpdateAsync(CarUpdateRequest request);
        Task DeleteAsync(int id);
        Task SetAvailabilityAsync(int carId, bool isAvailable);
    }
}
