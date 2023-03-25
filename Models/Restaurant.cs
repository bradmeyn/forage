using System;
using System.ComponentModel.DataAnnotations;

namespace Forage.Models
{
    public class Restaurant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNo { get; set; }

        public int UserId { get; set; }

        [Url]
        public string Website { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool VeganOptions { get; set; }

        [Required]
        public bool VegetarianOptions { get; set; }

        [Url]
        public string? ImageURL { get; set; }

        public ICollection<RestaurantCuisine> RestaurantCuisines { get; set; }
    }
}
