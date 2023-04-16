using Forage.Models;
using System.Collections.Generic;


namespace Forage.ViewModels
{
    public class DashboardViewModel
    {
        public Restaurant? Restaurant { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<Review> RecentReviews { get; set; }
        public List<Booking> BookingsToday { get; set; }
        public int BookingCount { get; set; }

        // Review Chart Values
        public int FiveStarReviews { get; set; } 
        public int FourStarReviews { get; set; }
        public int ThreeStarReviews { get; set; }
        public int TwoStarReviews { get; set; }
        public int OneStarReviews { get; set; }


    }


}
