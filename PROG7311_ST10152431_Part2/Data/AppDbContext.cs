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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Farmer)
                .WithMany(f => f.Products)
                .HasForeignKey(p => p.FarmerId);
        }
    }
}
