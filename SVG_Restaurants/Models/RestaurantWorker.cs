using System;
using System.Collections.Generic;

namespace SVG_Restaurants.Models
{
    public partial class RestaurantWorker
    {
        public int WorkerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int? RestaurantId { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
    }
}
