using System.ComponentModel.DataAnnotations;

namespace Forage.ViewModels
{
    public class EditReviewViewModel
    {
        public int ReviewId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Details { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
