using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACar.Areas.Identity.Data;
using RentACar.Models;

namespace RentACar.Data
{
    public class RentACarContext : IdentityDbContext<RentACarUser>
    {
        public RentACarContext(DbContextOptions<RentACarContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<RentACar.Models.Car> Cars { get; set; }
        public DbSet<UserCar> UserCars { get; set; }
    }
}
