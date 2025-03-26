﻿using CarRental.Data.Models;
using CarRental.Data.DTOs;

namespace CarRental.Interfaces
{
    public interface ICustomerService
    {
        Task RegisterAsync(RegisterCustomerRequest dto);
        Task<bool> VerifyEmailAsync(string email, string token);
        Task<Customer?> LoginAsync(string email, string password);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByEmailAsync(string email);
        Task UpdateProfileAsync(int customerId, UpdateCustomerRequest dto);
        Task DeleteCustomerAsync(int customerId);
        Task RestoreCustomerAsync(int customerId);
    }
}
