using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SVG_Restaurants.Models
{
    public partial class Reservation
    {
        public int ReservationId { get; set; }
        public int? CustomerId { get; set; }
        public int? RestaurantId { get; set; }


        [Display(Name = "Dining Area Preference")]
        [Required(ErrorMessage = "{0} is required.")]
        public int? AreaId { get; set; }

        [Display(Name = "Reservation Time")]
        [Required(ErrorMessage = "{0} is required.")]
        public DateTime? ReservationTiming { get; set; }

        [Display(Name = "Banquet Name")]
        public int? BanquetId { get; set; }

        [Display(Name = "Number of People")]
        [Required(ErrorMessage = "{0} is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Booking must have at least 1 person")]
        public int? NumberOfPeople { get; set; }
        public int? GuestId { get; set; }

        [Display(Name = "Baby Seats")]
        public int? HighChairs { get; set; }

        [Display(Name = "Special Notes")]
        public string? SpecialNotes { get; set; }

        public virtual DiningArea? Area { get; set; }
        public virtual Banquet? Banquet { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Guest? Guest { get; set; }
        public virtual Restaurant? Restaurant { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                // Calculate FullName based on Customer or Guest properties
                if (Customer != null)
                {
                    return $"{Customer.FirstName} {Customer.LastName}";
                }
                else if (Guest != null)
                {
                    return $"{Guest.FirstName} {Guest.LastName}";
                }
                else
                {
                    // Handle cases where both Customer and Guest are null
                    return "Unknown";
                }
            }
        }
    }
}
