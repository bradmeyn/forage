using Forage.Models;
using System.Collections.Generic;

namespace Forage.ViewModels
{
    public class RestaurantIndexViewModel
    {
        public IEnumerable<Restaurant> Restaurants { get; set; }
        public IEnumerable<CuisineType> Cuisines { get; set; }
        public IEnumerable<PricingOption> Pricing { get; set; }
        public IEnumerable<VenueType> Venues { get; set; }
        public string? SearchTerm { get; set; }
    }
}
