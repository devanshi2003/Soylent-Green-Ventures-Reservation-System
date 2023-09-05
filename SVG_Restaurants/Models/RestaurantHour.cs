using System;
using System.Collections.Generic;

namespace SVG_Restaurants.Models
{
    public partial class RestaurantHour
    {
        public int? RestaurantId { get; set; }
        public DateTime? ClosedDateTo { get; set; }
        public DateTime? ClosedDateFrom { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
    }
}
