using Forage.Data;
using Forage.Models;
using Forage.Services;
using Forage.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;


namespace Forage.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;
        private readonly UserManager<User> _userManager;

        public RestaurantController(
            ApplicationDbContext context, 
            IImageUploadService imageUploadService, 
            UserManager<User> userManager
            )
        {
            _context = context;
            _imageUploadService = imageUploadService;
            _userManager = userManager;
        }

        // GET: /restaurants
        [HttpGet("/restaurants")]
        public IActionResult Index()
        {
            var viewModel = new RestaurantIndexViewModel
            {
                Restaurants = _context.Restaurants.Include(r => r.Reviews).ToList(),
            };

            return View(viewModel);
        }

        // GET: /restaurants/{id}
        [HttpGet("/restaurants/{id}")]
        public IActionResult Detail(int id)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.Reviews)
                .ThenInclude(r => r.User)
                .Include(r => r.Address)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var viewModel = new RestaurantDetailViewModel
            {
                Restaurant = restaurant,
                Reviews = restaurant.Reviews.ToList(),
                AverageRating = restaurant.Reviews.Average(r => r.Rating) ?? 0,

                ReviewCount = restaurant.Reviews.Count(),
                Address = restaurant.Address,
                UserId = _userManager.GetUserId(User)
            };

            return View(viewModel);
        }

        // GET: /restaurants/new
        [HttpGet("/restaurants/new")]
        [Authorize(Roles = "Admin, Business")]
        public IActionResult Create()
        {
            var viewModel = new CreateRestaurantViewModel
            {
                
            };

            return View(viewModel);
        }

        // POST: /restaurants/new
        [HttpPost("/restaurants/new")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Business")]
        public async Task<IActionResult> Create(CreateRestaurantViewModel model)
        {
            if (ModelState.IsValid)
            {

                var restaurant = new Restaurant
                {
                    Name = model.Name,
                    UserId = _userManager.GetUserId(User),
                    Email = model.Email,
                    PhoneNo = model.PhoneNo,
                    Website = model.Website,
                    IsActive = true,
                    VeganOptions = model.VeganOptions,
                    VegetarianOptions = model.VegetarianOptions,
                    GlutenFreeOptions = model.GlutenFreeOptions,
                    VenueType = model.VenueType,
                    Cuisine = model.Cuisine,
                    OnlineBooking = model.OnlineBooking,
                    DineIn = model.DineIn,
                    TakeAway = model.TakeAway,
                    Pricing = model.Pricing,
                    Address = new Address
                    {
                        StreetNumber = model.StreetNumber,
                        StreetName = model.StreetName,
                        Suburb = model.Suburb,
                        State = model.State,
                        PostCode = model.PostCode
                    },

                };

                // Upload image to Cloudinary
                if (model.RestaurantImage != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.RestaurantImage.CopyToAsync(memoryStream);
                        var imageUrl = await _imageUploadService.UploadImageAsync(memoryStream.ToArray(), model.RestaurantImage.FileName);
                        restaurant.ImageURL = imageUrl;
                    }
                }
                else
                {
                    restaurant.ImageURL = "https://res.cloudinary.com/dzkirdhwv/image/upload/v1680255587/restaurant_ls6ypd.jpg";
                }

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
