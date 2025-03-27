using CarRental.Data.DTOs;
using CarRental.Data.Models;

namespace CarRental.Interfaces
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<IEnumerable<Car>> GetAvailableAsync();
        Task<Car?> GetByIdAsync(int id);
        Task AddAsync(CarCreateRequest request);
        Task UpdateAsync(CarUpdateRequest request);
        Task DeleteAsync(int id);
        Task SetAvailabilityAsync(int carId, bool isAvailable);
    }
}
