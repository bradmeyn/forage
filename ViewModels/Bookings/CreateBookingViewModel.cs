using System.ComponentModel.DataAnnotations;
using Forage.Models;

namespace Forage.ViewModels
{
    public class CreateBookingViewModel
    {
        [Required]
        public int RestaurantId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public string Time { get; set; }

        [Required]
        [Range(1, 8)]
        public int PartySize { get; set; }

        public string RestaurantName { get; set; }

        public Restaurant? Restaurant { get; set; }

    }
}