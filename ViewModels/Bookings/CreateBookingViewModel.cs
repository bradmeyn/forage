using System.ComponentModel.DataAnnotations;
using Forage.Models;

namespace Forage.ViewModels
{
    public class CreateBookingViewModel
    {
        [Required]
        public int RestaurantId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BookingStart { get; set; }

        [Required]
        [Range(1, 10)]
        public int PartySize { get; set; }

        public string RestaurantName { get; set; }

    }
}