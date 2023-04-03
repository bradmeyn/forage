using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Forage.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Address? Address { get; set; }

        [Url]
        public string? ProfileURL { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; }
    }

}

