using Forage.Models;
using System.Collections.Generic;


namespace Forage.ViewModels
{
    public class RestaurantDetailViewModel
    {
        public Restaurant Restaurant { get; set; }
        public double AverageRating { get; set; }
        public int? ReviewCount { get; set; }
        public string UserId { get; set; }

    }
}
