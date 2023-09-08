using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SVG_Restaurants.Models
{
    public partial class RestaurantWorker
    {
        public int WorkerId { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? Username { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "{0} is required.")]
        public string? Password { get; set; }
        public int? RestaurantId { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
    }
}
