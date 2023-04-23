using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forage.Models
{
    public enum BookingStatus
    {
        Confirmed,
        Cancelled
    }
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
        public Restaurant? Restaurant { get; set; }

        [Required]
        public DateTime BookingStart { get; set; }
        
        public BookingStatus Status { get; set; }

        [Required]
        [Range(1, 10)]
        public int? PartySize { get; set; }

    }
}
