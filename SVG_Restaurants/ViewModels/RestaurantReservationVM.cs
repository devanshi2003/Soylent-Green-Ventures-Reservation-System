using System;
using SVG_Restaurants.Models;

namespace SVG_Restaurants.ViewModels
{
	public class RestaurantReservationVM
	{
		public RestaurantReservationVM()
		{
		}

		public List<Reservation> reservations { get; set; }
		public Restaurant theRestaurant { get; set; }
		public int RestaurantId { get; set; }
        public string? nameSearch { get; set; }

    }
}

