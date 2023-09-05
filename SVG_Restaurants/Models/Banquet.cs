using System;
using System.Collections.Generic;

namespace SVG_Restaurants.Models
{
    public partial class Banquet
    {
        public Banquet()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int BanquetId { get; set; }
        public string? BanquetName { get; set; }
        public int? RestaurantId { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
