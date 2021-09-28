using RentACar.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class UserCar
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public RentACarUser RentACarUser { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required(ErrorMessage = "Please enter number of days")]
        public int Days { get; set; }

        public bool Returned { get; set; }
    }
}
