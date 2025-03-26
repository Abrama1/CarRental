using CarRental.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data
{
    public class CarRentalDbContext : DbContext
    {
        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : base(options) { }

        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Rental> Rentals => Set<Rental>();
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rental relationships
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Car)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.CarId);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.CustomerId);
        }
    }
}
