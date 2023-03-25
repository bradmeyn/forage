using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forage.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        [Required]
        public int AvailabilityId { get; set; }

        [ForeignKey("AvailabilityId")]
        public Availability Availability { get; set; }

        [Required]
        public DateTime BookingStart { get; set; }

        [Required]
        [Range(1, 100)]
        public int PartySize { get; set; }

    }
}
