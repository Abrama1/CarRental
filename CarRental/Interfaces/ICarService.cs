using CarRental.Data.Models;

namespace CarRental.Interfaces
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<IEnumerable<Car>> GetAvailableAsync();
        Task<Car?> GetByIdAsync(int id);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);
        Task SetAvailabilityAsync(int carId, bool isAvailable);
    }
}
