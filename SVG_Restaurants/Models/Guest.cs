using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SVG_Restaurants.Models
{
    public partial class Guest
    {
        public Guest()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int GuestId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        [Display(Name = "Customer Name")]
        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
