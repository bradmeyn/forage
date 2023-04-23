using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Forage.Models;
using System.ComponentModel.DataAnnotations;


namespace Forage.ViewModels
{
    public class CreateRestaurantViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNo { get; set; }

        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Display(Name = "Vegan Options")]
        public bool VeganOptions { get; set; }

        [Display(Name = "Vegetarian Options")]
        public bool VegetarianOptions { get; set; }

        [Display(Name = "Gluten-Free Options")]
        public bool GlutenFreeOptions { get; set; }

        [Display(Name = "Venue Type")]
        public VenueType VenueType { get; set; }

        [Display(Name = "Online Booking")]
        public bool OnlineBooking { get; set; }

        [Display(Name = "Dine In")]
        public bool DineIn { get; set; }

        [Display(Name = "Take Away")]
        public bool TakeAway { get; set; }

        [Display(Name = "Restaurant Image")]
        public IFormFile? RestaurantImage { get; set; }

        [Display(Name = "Pricing")]
        public PricingOption Pricing { get; set; }

        [Display(Name = "Street Number")]
        [Required(ErrorMessage = "Street number is required")]
        public int StreetNumber { get; set; }

        [Display(Name = "Street Name")]
        [Required(ErrorMessage = "Street name is required")]
        public string StreetName { get; set; }

        [Display(Name = "Suburb")]
        [Required(ErrorMessage = "Suburb is required")]
        public string Suburb { get; set; }

        [Display(Name = "State / Territory")]
        [Required(ErrorMessage = "State or territory is required")]
        public State State { get; set; }

        [Display(Name = "Postcode")]
        [Required(ErrorMessage = "Postcode is required")]
        public int PostCode { get; set; }

        [Display(Name = "Cuisine")]
        public CuisineType Cuisine { get; set; }

        // Days Open
        public bool OpenMonday { get; set; }
        public bool OpenTuesday { get; set; }
        public bool OpenWednesday { get; set; }
        public bool OpenThursday { get; set; }
        public bool OpenFriday { get; set; }
        public bool OpenSaturday { get; set; }
        public bool OpenSunday { get; set; }

        // Opening/Closing times
        public TimeOnly? WeekdayOpen { get; set; }
        public TimeOnly? WeekdayClose { get; set; }
        public TimeOnly? WeekendOpen { get; set; }
        public TimeOnly? WeekendClose { get; set; }


    }
}
