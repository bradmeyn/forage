using Forage.Models;
using System.Collections.Generic;

namespace Forage.ViewModels
{
    public class RestaurantIndexViewModel
    {
        // Restaurant properties
        public IEnumerable<Restaurant> Restaurants { get; set; }
        public IEnumerable<CuisineType> Cuisines { get; set; }
        public IEnumerable<PricingOption> Pricing { get; set; }
        public IEnumerable<VenueType> Venues { get; set; }

        // Search & Filter properties
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; }
        public List<CuisineType> SelectedCuisines { get; set; } 
        public List<PricingOption> SelectedPricing { get; set; }
        public bool Vegetarian { get; set; }
        public bool Vegan { get; set; }
        public bool GlutenFree { get; set; }
        public bool DineIn { get; set; }
        public bool TakeAway { get; set; }
        public bool Booking { get; set; }

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        public int TotalRestaurants { get; set; }

    }
}
