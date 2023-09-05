using System;
using System.Collections.Generic;

namespace SVG_Restaurants.Models
{
    public partial class Reservation
    {
        public int ReservationId { get; set; }
        public int? CustomerId { get; set; }
        public int? RestaurantId { get; set; }
        public int? AreaId { get; set; }
        public DateTime? ReservationTiming { get; set; }
        public int? BanquetId { get; set; }
        public int? NumberOfPeople { get; set; }

        public virtual DiningArea? Area { get; set; }
        public virtual Banquet? Banquet { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
    }
}
