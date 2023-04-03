using System.ComponentModel.DataAnnotations;

namespace Forage.Models
{
    public enum State
    {
        ACT,
        NSW,
        NT,
        QLD,
        SA,
        TAS,
        VIC,
        WA
    }
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Unit Number")]
        public int? UnitStreetNumber { get; set; }

        [Required]
        [Display(Name = "Street Number")]
        public int? StreetNumber { get; set; }

        [Required]
        [Display(Name = "Street Name")]
        public string? StreetName { get; set; }

        [Required]
        [Display(Name = "Suburb")]
        public string? Suburb { get; set; }

        [Required]
        [Display(Name = "State / Territory")]
        public State? State { get; set; }

        [Required]
        [Display(Name = "Post Code")]
        public int? PostCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }  
}
