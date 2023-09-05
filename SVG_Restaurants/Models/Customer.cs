using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SVG_Restaurants.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int CustomerId { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Username")]
        public string? Username { get; set; }

        [Display(Name = "Password")]
        public string? Password { get; set; }
        public int? LoyaltyPoints { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
