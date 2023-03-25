using System;
using System.ComponentModel.DataAnnotations;

namespace Forage.Models
{
    public class Cuisine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public ICollection<RestaurantCuisine> RestaurantCuisines { get; set; }
    }
}

