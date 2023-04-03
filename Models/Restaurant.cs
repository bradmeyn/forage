using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;


namespace Forage.Models
{
    public enum VenueType
    {
        Restaurant,
        Cafe,
        Bar,
        Other
    }

    public enum PricingOption
    {
        CheapEats,
        BudgetFriendly,
        MidRange,
        FineDining
    }

    public enum CuisineType
    {
        Australian,
        BBQ,
        Cafe,
        Chinese,
        Indian,
        Italian,
        Thai,
        Vietnamese,
    }

    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNo { get; set; }

        public string? UserId { get; set; }

        public string? Website { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        public Restaurant()
        {
            CreatedAt = DateTime.UtcNow;
        }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool VeganOptions { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool VegetarianOptions { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool GlutenFreeOptions { get; set; }
        
        [Required]
        public VenueType? VenueType { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool OnlineBooking { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool DineIn { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool TakeAway { get; set; }

        public string? ImageURL { get; set; }

        public Address? Address { get; set; }

        public PricingOption? Pricing { get; set; }

        public CuisineType? Cuisine { get; set; }

        public ICollection<Review> Reviews { get; set; }

    }
}
