using Forage.Models;
using System.Collections.Generic;

namespace Forage.ViewModels
{
    public class AdminViewModel
    {
        public List<User> Users { get; set; }
        public List<Restaurant> Restaurants { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
