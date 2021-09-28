using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentACar.Areas.Identity.Data;
using RentACar.Data;

[assembly: HostingStartup(typeof(RentACar.Areas.Identity.IdentityHostingStartup))]
namespace RentACar.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
               /* services.AddDbContext<RentACarContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("RentACarContextConnection")));*/

                /*services.AddDefaultIdentity<RentACarUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<RentACarContext>();*/
            });
        }
    }
}