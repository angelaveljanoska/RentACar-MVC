using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentACar.Areas.Identity.Data;
using RentACar.Data;
using RentACar.Models;
using RentACar.ViewModels;

namespace RentACar.Controllers
{
    public class CarsController : Controller
    {
        private readonly RentACarContext _context;
        private readonly UserManager<RentACarUser> _userManager;
        private readonly IHostingEnvironment webHostEnvironment;

        public CarsController(RentACarContext context, UserManager<RentACarUser> userManager, IHostingEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.AvailableSortParm = String.IsNullOrEmpty(sortOrder) ? "available_desc" : "";
            ViewBag.BrandSortParm = String.IsNullOrEmpty(sortOrder) ? "brand_desc" : "";
            ViewBag.PriceSortParm = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            var cars = from c in _context.Cars
                       select c;
            switch (sortOrder)
            {
                case "brand_desc":
                    cars = cars.OrderByDescending(c => c.Brand);
                    break;
                case "price_desc":
                    cars = cars.OrderByDescending(c => c.PricePerDay);
                    break;
                case "available_desc":
                    cars = cars.OrderByDescending(c => c.Availability);
                    break;
                default:
                    cars = cars.OrderBy(c => c.PricePerDay);
                    break;
            }
            return View(cars.ToList());
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public void AddPicture(string base64, int modelId) {
            Console.WriteLine("base64: " + base64 + " modelId: " + modelId);
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Model, string Brand, int YearOfManufacture, int PricePerDay, bool Availability, IFormFile Picture)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(Picture);

                Car car = new Car
                {
                    Model = Model,
                    Brand = Brand,
                    YearOfManufacture = YearOfManufacture,
                    PricePerDay = PricePerDay,
                    Availability = Availability,
                    Picture = uniqueFileName,
                };
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        private string UploadedFile(/*NewCarViewModel model*/ IFormFile Picture)
        {
            string uniqueFileName = null;

            if (Picture != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Picture.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Picture.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            NewCarViewModel newCarViewModel = new NewCarViewModel
            {
                Id = car.Id,
                Model = car.Model,
                Brand = car.Brand,
                YearOfManufacture = car.YearOfManufacture,
                PricePerDay = car.PricePerDay,
                Availability = car.Availability,
        };
            return View(newCarViewModel);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, string Model, string Brand, int YearOfManufacture, int PricePerDay, IFormFile Picture)
        {
            var car = await _context.Cars.Where(c => c.Id == Id).FirstOrDefaultAsync();
            // TODO: put picture logic here too
            if (ModelState.IsValid)
            {
                car.Model = Model;
                car.Brand = Brand;
                car.YearOfManufacture = YearOfManufacture;
                car.PricePerDay = PricePerDay;
                try
                {
                    string uniqueFileName = UploadedFile(Picture);
                    if(uniqueFileName!=null)
                    {
                        car.Picture = uniqueFileName;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _context.Entry(car).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View();
        }

        // GET: Cars/Rent/5
        public async Task<IActionResult> Rent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            var viewModel = new RentCarViewModel {
                Car = car,
                Days = 0,
            };
            return View(viewModel);
        }

        public async Task<IActionResult> MyRents()
        {
            RentACarUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                throw new Exception("Error!");
            }

            var cars = await _context.Cars.Include(c => c.Users).ThenInclude(u => u.RentACarUser).Include(c => c.Users).ThenInclude(u => u.Car).ToListAsync();
            var newCars = new List<Car>();
            foreach(Car car in cars)
            {
                foreach(UserCar relation in car.Users)
                {
                    if(!relation.Returned && relation.UserId==user.Id)
                    {
                        newCars.Add(relation.Car);
                    }
                }
            }
            return View(newCars);
        }

        public async Task<IActionResult> ReturnCar(int? id)
        {
            RentACarUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                throw new Exception("Error!");
            }

            var car = await _context.Cars.Where(c => c.Id == id).Include(c => c.Users).ThenInclude(u => u.Car).FirstOrDefaultAsync();
            _context.Entry(car).State = EntityState.Modified;
            foreach (UserCar relation in car.Users)
            {
                if (relation.UserId == user.Id && car.Id==relation.CarId && !relation.Returned)
                {
                    relation.Returned = true;
                    car.Availability = true;
                    _context.Entry(relation).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RentCar(Car car, int? days)
        {
            car = await _context.Cars.FirstOrDefaultAsync(c => c.Id==car.Id);
            if (car == null || days == null)
            {
                throw new Exception("Error!");
            }
            RentACarUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null || car == null)
            {
                throw new Exception("Error!");
            }

            var rent = new UserCar
            {
                RentACarUser = user,
                UserId = user.Id,
                Car = car,
                CarId = car.Id,
                Days = (int)days,
                Returned = false,
            };
            _context.Add(rent);
            car.Availability = false;
            _context.Update(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
