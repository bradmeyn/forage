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
        public DbSet<RestaurantCuisine> RestaurantCuisines { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Availability> Availabilities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Restaurant and Cuisine
            modelBuilder.Entity<RestaurantCuisine>()
                .HasKey(rc => new { rc.RestaurantId, rc.CuisineId });

            modelBuilder.Entity<RestaurantCuisine>()
                .HasOne(rc => rc.Restaurant)
                .WithMany(r => r.RestaurantCuisines)
                .HasForeignKey(rc => rc.RestaurantId);

            modelBuilder.Entity<RestaurantCuisine>()
                .HasOne(rc => rc.Cuisine)
                .WithMany(c => c.RestaurantCuisines)
                .HasForeignKey(rc => rc.CuisineId);
        }
    }
}
