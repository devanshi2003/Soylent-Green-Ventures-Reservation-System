using System;
using System.Collections.Generic;

namespace SVG_Restaurants.Models
{
    public partial class Restaurant
    {
        public Restaurant()
        {
            Banquets = new HashSet<Banquet>();
            DiningAreas = new HashSet<DiningArea>();
            Reservations = new HashSet<Reservation>();
            RestaurantWorkers = new HashSet<RestaurantWorker>();
        }

        public int RestaurantId { get; set; }
        public string? RestaurantAddress { get; set; }
        public string? RestaurantName { get; set; }
        public int? SeatCapacity { get; set; }

        public virtual ICollection<Banquet> Banquets { get; set; }
        public virtual ICollection<DiningArea> DiningAreas { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<RestaurantWorker> RestaurantWorkers { get; set; }
    }
}
