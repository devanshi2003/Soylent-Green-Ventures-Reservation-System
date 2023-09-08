using System;
using SVG_Restaurants.Models;

namespace SVG_Restaurants.ViewModels
{
	public class RestaurantReservationVM
	{
		public RestaurantReservationVM()
		{
		}

		public Reservation TheReservation { get; set; }
		public RestaurantVM restaurantVM { get; set; }
	}
}

