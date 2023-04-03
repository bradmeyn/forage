using Forage.Models;
using System.Collections.Generic;


namespace Forage.ViewModels
{
    public class RestaurantDetailViewModel
    {
        public Restaurant Restaurant { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public double AverageRating { get; set; }
        public int? ReviewCount { get; set; }
        public Address Address { get; set; }
        public string UserId { get; set; }

        public CreateReviewViewModel CreateReview { get; set; }

    }
}
