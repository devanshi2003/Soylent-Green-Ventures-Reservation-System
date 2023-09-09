using SVG_Restaurants.Models;
using System;
namespace SVG_Restaurants.ViewModels
{
    public class CustomerDetailsVM
    {
        public int CustomerID { get; set; }
        public IEnumerable<Reservation>? Reservations { get; set; }
    }
}


