using System;
using System.Collections.Generic;

namespace SVG_Restaurants.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int? LoyaltyPoints { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
