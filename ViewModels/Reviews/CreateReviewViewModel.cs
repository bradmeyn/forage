using System.ComponentModel.DataAnnotations;

namespace Forage.ViewModels
{
    public class CreateReviewViewModel
    {
        [Required]
        public int RestaurantId { get; set; }

        public string RestaurantName { get; set; }

        [Required(ErrorMessage = "Please enter a title for your review.")]
        [StringLength(100, ErrorMessage = "Title should not exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please provide some details for your review.")]
        [StringLength(1000, ErrorMessage = "Details should not exceed 1000 characters.")]
        public string Details { get; set; }

        [Required(ErrorMessage = "Please provide a rating between 1 and 5.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
    }
}
