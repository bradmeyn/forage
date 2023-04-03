using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Forage.Models
{
    public class Availability
    {

        [Key]
        public int? AvailabilityId { get; set; }

        [Required]
        public int? RestaurantId { get; set; }

        [Required]
        public DateTime? TimeSlot { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Filled { get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }

        [Required]
        public int? MaxPartySize { get; set;}
    }
}
