using System;
using System.Collections.Generic;

namespace SVG_Restaurants.Models
{
    public partial class DiningArea
    {
        public DiningArea()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int AreaId { get; set; }
        public string? Area { get; set; }
        public int? RestaurantId { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
