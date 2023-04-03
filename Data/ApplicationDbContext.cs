using Forage.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forage.Data
{
    public class ApplicationDbContext : IdentityDbContext<User> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Availability> Availabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<Restaurant>()
            //     .HasMany(r => r.Reviews)
            //     .WithOne(r => r.Restaurant)
            //     .HasForeignKey(r => r.RestaurantId);

            // modelBuilder.Entity<Restaurant>()
            //     .HasMany(r => r.Bookings)
            //     .WithOne(r => r.Restaurant)
            //     .HasForeignKey(r => r.RestaurantId);

            // modelBuilder.Entity<Restaurant>()
            //     .HasMany(r => r.Availabilities)
            //     .WithOne(r => r.Restaurant)
            //     .HasForeignKey(r => r.RestaurantId);

        }
    }
}
