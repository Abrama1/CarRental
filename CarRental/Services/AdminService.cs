using CarRental.Data;
using CarRental.Data.Models;
using CarRental.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    public class AdminService : IAdminService
    {
        private readonly CarRentalDbContext _context;
        private readonly TokenService _tokenService;

        public AdminService(CarRentalDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var admin = await _context.AdminUsers.FirstOrDefaultAsync(a => a.Username == username);
            if (admin == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
            return valid ? _tokenService.CreateToken(admin.Id.ToString(), admin.Username, admin.Role) : null;
        }
    }
}
