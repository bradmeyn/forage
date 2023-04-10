using Forage.Models;
using System.Collections.Generic;

namespace Forage.ViewModels
{
    public class HomeViewModel
    {
        // Restaurant properties
        public IEnumerable<Restaurant> TopRatedRestaurants { get; set; }
        public IEnumerable<Restaurant> NewestRestaurants { get; set; }
    }
}