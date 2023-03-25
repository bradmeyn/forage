using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forage.Models
{
    public class Availability
{
    public Availability()
    {
        Status = "Pending"; // Set a default value for Status
    }

    [Key]
    public int AvailabilityId { get; set; }

    [Required]
    public int RestaurantId { get; set; }

    [Required]
    public DateTime Timeslot { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }

    [ForeignKey("RestaurantId")]
    public Restaurant Restaurant { get; set; }
}

}
