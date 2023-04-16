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

        public User? User { get; set; }

        [Url]
        public string? Website { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

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
        public VenueType VenueType { get; set; }

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

        public PricingOption Pricing { get; set; }

        public CuisineType Cuisine { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public int Capacity { get; set; }

        public bool OpenSunday { get; set; }
        public TimeOnly? SundayOpenTime { get; set; }
        public TimeOnly? SundayCloseTime { get; set; }

        public bool OpenMonday { get; set; }
        public TimeOnly? MondayOpenTime { get; set; }
        public TimeOnly? MondayCloseTime { get; set; }

        public bool OpenTuesday { get; set; }
        public TimeOnly? TuesdayOpenTime { get; set; }
        public TimeOnly? TuesdayCloseTime { get; set; }

        public bool OpenWednesday { get; set; }
        public TimeOnly? WednesdayOpenTime { get; set; }
        public TimeOnly? WednesdayCloseTime { get; set; }

        public bool OpenThursday { get; set; }
        public TimeOnly? ThursdayOpenTime { get; set; }
        public TimeOnly? ThursdayCloseTime { get; set; }

        public bool OpenFriday { get; set; }
        public TimeOnly? FridayOpenTime { get; set; }
        public TimeOnly? FridayCloseTime { get; set; }

        public bool OpenSaturday { get; set; }
        public TimeOnly? SaturdayOpenTime { get; set; }
        public TimeOnly? SaturdayCloseTime { get; set; }


    }
}
