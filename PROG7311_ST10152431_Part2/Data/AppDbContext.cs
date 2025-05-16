using Microsoft.EntityFrameworkCore;
using PROG7311_ST10152431_Part2.Models;
using PROG7311_ST10152431_Part2.Data;

namespace PROG7311_ST10152431_Part2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Farmer> Farmers { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Farmers
            modelBuilder.Entity<Farmer>().HasData(
                new Farmer { FarmerId = 1, FarmerName = "John", FarmerSurname = "Doe", FarmerPassword = "pass123" },
                new Farmer { FarmerId = 2, FarmerName = "Jane", FarmerSurname = "Smith", FarmerPassword = "pass456" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Maize", ProductCategory = "Grain", ProductionDate = new DateTime(2024, 1, 10), ProductPrice = 100, FarmerId = 1 },
                new Product { ProductId = 2, ProductName = "Tomato", ProductCategory = "Vegetable", ProductionDate = new DateTime(2024, 2, 15), ProductPrice = 50, FarmerId = 2 }
            );

            // Seed Employees (demo)
            modelBuilder.Entity<Employee>().HasData(
                new Employee { EmployeeId = 1, EmployeeName = "AgriEnergy" }
            );
        }

    }
}
