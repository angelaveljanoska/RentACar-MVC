using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RentACar.Models;

namespace RentACar.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the RentACarUser class
    public class RentACarUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Please enter your age")]
        [PersonalData]
        [Column]
        public int Age { get; set; }

        [Required(ErrorMessage = "Please enter your driver's licence")]
        [PersonalData]
        [Column]
        public string Licence { get; set; }

        public ICollection<UserCar> Cars { get; set; }

    }
}
