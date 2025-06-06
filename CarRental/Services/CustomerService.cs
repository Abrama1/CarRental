﻿using System.Security.Cryptography;
using CarRental.Data;
using CarRental.Data.Models;
using CarRental.Data.DTOs;
using CarRental.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CarRentalDbContext _context;
        private readonly IEmailService _emailService;
        private readonly TokenService _tokenService;

        public CustomerService(CarRentalDbContext context, IEmailService emailService, TokenService tokenService)
        {
            _context = context;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task RegisterAsync(RegisterCustomerRequest dto)
        {
            var exists = await _context.Customers.AnyAsync(c => c.Email == dto.Email);
            if (exists) throw new Exception("Email already in use");

            var tokenBytes = RandomNumberGenerator.GetBytes(32);
            var verificationToken = new Random().Next(100000, 999999).ToString();

            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                JoinDate = DateTime.UtcNow,
                IsVerified = false,
                VerificationToken = verificationToken,
                TokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            await _emailService.SendVerificationEmailAsync(customer.Email, verificationToken);

        }

        public async Task<bool> VerifyEmailAsync(string email, string token)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null || customer.IsDeleted) return false;
            if (customer.VerificationToken != token || customer.TokenExpiry < DateTime.UtcNow) return false;

            customer.IsVerified = true;
            customer.VerificationToken = null;
            customer.TokenExpiry = null;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email && !c.IsDeleted);

            if (customer == null || !customer.IsVerified)
                return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, customer.PasswordHash);
            return valid
                ? _tokenService.CreateToken(customer.Id.ToString(), customer.Email, customer.Role)
                : null;
        }

        public async Task<CustomerResponse?> GetByIdAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Rentals)
                    .ThenInclude(r => r.Car)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (customer == null) return null;

            return new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                JoinDate = customer.JoinDate,
                Role = customer.Role,
                Rentals = customer.Rentals.Select(r => new RentalResponse
                {
                    Id = r.Id,
                    CarId = r.CarId,
                    CarMake = r.Car?.Make ?? "",
                    CarModel = r.Car?.Model ?? "",
                    CustomerId = r.CustomerId,
                    CustomerName = customer.Name,
                    CustomerPhone = customer.Phone,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status.ToString()
                }).ToList()
            };
        }


        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email && !c.IsDeleted);
        }

        public async Task UpdateProfileAsync(int customerId, UpdateCustomerRequest dto)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null || customer.IsDeleted) return;

            customer.Name = dto.Name;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null || customer.IsDeleted) return;

            customer.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null || !customer.IsDeleted) return;

            customer.IsDeleted = false;
            await _context.SaveChangesAsync();
        }
    }
}