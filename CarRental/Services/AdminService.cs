using CarRental.Data;
using CarRental.Data.Models;
using CarRental.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    public class AdminService : IAdminService
    {
        private readonly CarRentalDbContext _context;

        public AdminService(CarRentalDbContext context)
        {
            _context = context;
        }

        public async Task<AdminUser?> LoginAsync(string username, string password)
        {
            var admin = await _context.AdminUsers.FirstOrDefaultAsync(a => a.Username == username);
            if (admin == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
            return valid ? admin : null;
        }
    }
}
