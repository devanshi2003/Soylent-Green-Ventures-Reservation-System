using SVG_Restaurants.Models;

namespace SVG_Restaurants.ViewModels
{
    public class RedeemViewModel
    {
        public int CustomerId { get; set; }
        public int pointsToRedeem { get; set; }

        public Customer customer { get; set; }
    }
}
