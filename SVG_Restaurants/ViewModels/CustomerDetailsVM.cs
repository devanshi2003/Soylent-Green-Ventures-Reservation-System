using SVG_Restaurants.Models;
using System;
namespace SVG_Restaurants.ViewModels
{
    public class CustomerDetailsVM
    {
        public int CustomerId { get; set; }
        public IEnumerable<Reservation>? Reservations { get; set; }
    }
}


