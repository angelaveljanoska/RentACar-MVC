using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [Required]
        [Display(Name = "Year Of Manufacture")]
        public int YearOfManufacture { get; set; }

        [Required]
        [Display(Name = "Price per day")]
        public int PricePerDay { get; set; }

        [Required]
        [Display(Name = "Availability")]
        public bool Availability { get; set; }

        [Required(ErrorMessage = "Please upload picture of the car")]
        public string Picture { get; set; }

        public ICollection<UserCar> Users { get; set; }

    }
}
