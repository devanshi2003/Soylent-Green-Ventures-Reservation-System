using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SVG_Restaurants.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int CustomerId { get; set; }


        [Display(Name = "First Name")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "{0} is required.")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? Username { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? Password { get; set; }

        [NotMapped]
        public string? NewPassword { get; set; }

        [Display(Name = "Customer Name")]
        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public int? LoyaltyPoints { get; set; }

        public string? Status { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
