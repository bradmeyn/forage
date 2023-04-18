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
using Microsoft.EntityFrameworkCore;
using System.Linq;
using PagedList;


namespace Forage.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RestaurantController> _logger;

        public RestaurantController(
            ApplicationDbContext context, 
            IImageUploadService imageUploadService, 
            UserManager<User> userManager,
            ILogger<RestaurantController> logger
            )
        {
            _context = context;
            _imageUploadService = imageUploadService;
            _userManager = userManager;
            _logger = logger;
        }

        // Restaurant Index
        // GET: /restaurants
        // Access: Public
        [HttpGet("/restaurants")]
        public IActionResult Index(RestaurantIndexViewModel filters)
        {
            // Get all restaurants with their reviews
            var query = _context.Restaurants.Include(r => r.Reviews).AsQueryable();

            // Apply filters

            // Apply search query
            if (!string.IsNullOrEmpty(filters.SearchQuery))
            {
                query = query.Where(r => r.Name.Contains(filters.SearchQuery.ToLower()));
            }

            // Apply cuisine filters
            if (filters.SelectedCuisines != null && filters.SelectedCuisines.Any())
            {
                query = query.Where(r => filters.SelectedCuisines.Contains(r.Cuisine));
            }

            // Apply pricing filters
            if (filters.SelectedPricing != null && filters.SelectedPricing.Any())
            {
                query = query.Where(r => filters.SelectedPricing.Contains(r.Pricing));
            }

            // Apply dietary filters
            if (filters.Vegetarian)
            {
                query = query.Where(r => r.VegetarianOptions);
            }  

            // Dietary options: vegetarian, vegan, gluten-free
            if (filters.Vegan)
            {
                query = query.Where(r => r.VeganOptions);
            }

            if (filters.GlutenFree)
            {
                query = query.Where(r => r.GlutenFreeOptions);
            }

            // Venue options: dine-in, take-away, online booking
            if (filters.DineIn)
            {
                query = query.Where(r => r.DineIn);
            }

            if (filters.TakeAway)
            {
                query = query.Where(r => r.TakeAway);
            }

            if (filters.Booking)
            {
                query = query.Where(r => r.OnlineBooking);
            }

            // Apply sorting
            switch (filters.SortBy)
            {
                case "name_asc":
                    query = query.OrderBy(r => r.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(r => r.Name);
                    break;
                case "rating_asc":
                    query = query.OrderBy(r => r.Reviews.Any() ? r.Reviews.Average(rv => rv.Rating) : 0);
                    break;
                case "rating_desc":
                    query = query.OrderByDescending(r => r.Reviews.Any() ? r.Reviews.Average(rv => rv.Rating) : 0);
                    break;
                case "newest":
                    query = query.OrderByDescending(r => r.CreatedAt);
                    break;
                default:
                    break;
            }

            // Get the total number of restaurants before applying pagination
            filters.TotalRestaurants = query.Count();

            // Apply pagination
            int skip = (filters.CurrentPage - 1) * filters.PageSize;
            int take = filters.PageSize;
            query = query.Skip(skip).Take(take);

            // Create the view model
            var viewModel = new RestaurantIndexViewModel
            {
                Restaurants = query.ToList(),
                // Pass the filters back to the view model
                SearchQuery = filters.SearchQuery,
                SortBy = filters.SortBy,
                SelectedCuisines = filters.SelectedCuisines ?? new List<CuisineType>(),
                SelectedPricing = filters.SelectedPricing ?? new List<PricingOption>(),
                Vegetarian = filters.Vegetarian,
                Vegan = filters.Vegan,
                GlutenFree = filters.GlutenFree,
                DineIn = filters.DineIn,
                TakeAway = filters.TakeAway,
                Booking = filters.Booking,
                CurrentPage = filters.CurrentPage,
                PageSize = filters.PageSize,
                TotalRestaurants = filters.TotalRestaurants
            };

            return View(viewModel);
        }


        // Restaurant Detail
        // GET: /restaurants/{id}
        // Access: Public
        [HttpGet("/restaurants/{id}")]
        public IActionResult Detail(int id)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.Reviews.OrderByDescending (r => r.CreatedAt))
                .ThenInclude(r => r.User)
                .Include(r => r.Address)
                .FirstOrDefault(r => r.Id == id);

                var query = _context.Restaurants
    .Include(r => r.Reviews)
    .Select(r => new
    {
        Restaurant = r,
        Reviews = r.Reviews.OrderByDescending(review => review.CreatedAt).Take(5)
    })
    .AsQueryable();

            if(restaurant == null)
            {
                TempData["Error"] = "Restaurant not found";
                return RedirectToAction("Index", "Restaurant");
            }

            var viewModel = new RestaurantDetailViewModel
            {
                Restaurant = restaurant,
                AverageRating = restaurant.Reviews?.Select(r => r.Rating).DefaultIfEmpty().Average() ?? 0,
                ReviewCount = restaurant.Reviews.Count(),
                UserId = _userManager.GetUserId(User)
            };

            return View(viewModel);
        }

        // Get new restaurant form
        // GET: /restaurants/new
        // Access: Admin, Business
        [HttpGet("/restaurants/new")]
        public IActionResult Create()
        {
            // Check if user is logged in
            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to create a restaurant";
                return RedirectToAction("Login", "Account");
            }

            // Check if user if basic (foodie) account
            if (User.IsInRole("Basic"))
            {
                TempData["Error"] = "Foodie accounts cannot add restaurants";
                return RedirectToAction("Index", "Restaurant");
            }

            var viewModel = new CreateRestaurantViewModel
            {

            };

            return View(viewModel);
        }

        // Create new restaurant route
        // POST: /restaurants/new
        // Access: Admin, Business
        [HttpPost("/restaurants/new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRestaurantViewModel model)
        {
            // Check if user is logged in
            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to create a restaurant";
                return RedirectToAction("Login", "Account");
            }

            // Check if user if basic (foodie) account
            if ( User.IsInRole("Basic"))
            {
                 TempData["Error"] = "Foodie accounts cannot add restaurants";
                return RedirectToAction("Index", "Restaurant");
            }

            // Restaurant is open on the weekend
            if(model.OpenSaturday || model.OpenSunday) {

                // Check both weekend opening and closing times are not null
                if(model.WeekendOpen == null || model.WeekendClose == null) {
                    TempData["Error"] = "Please enter opening and closing times for the weekend";
                    return View(model);
                }

                // Check opening time is before closing time
                if(model.WeekendOpen >= model.WeekendClose) {
                    TempData["Error"] = "Opening time must be before closing time";
                    return View(model);
                }
            }

            // Restaurant is open on the weekdays
            if(model.OpenMonday || model.OpenTuesday || model.OpenWednesday || model.OpenThursday || model.OpenFriday) {
                    
                    // Check both weekday opening and closing times are not null
                    if(model.WeekdayOpen == null || model.WeekdayClose == null) {
                        TempData["Error"] = "Please enter opening and closing times for the weekdays";
                        return View(model);
                    }
    
                    // Check opening time is before closing time
                    if(model.WeekdayOpen >= model.WeekdayClose) {
                        TempData["Error"] = "Opening time must be before closing time";
                        return View(model);
                    }
            }

            if (!model.OpenSaturday && !model.OpenSunday && !model.OpenMonday && !model.OpenTuesday && !model.OpenWednesday && !model.OpenThursday && !model.OpenFriday) {
                TempData["Error"] = "Please select at least one day of the week";
                return View(model);
            }
            
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

                    OpenSunday = model.OpenSunday,
                    OpenMonday = model.OpenMonday,
                    OpenTuesday = model.OpenTuesday,
                    OpenWednesday = model.OpenWednesday,
                    OpenThursday = model.OpenThursday,
                    OpenFriday = model.OpenFriday,
                    OpenSaturday = model.OpenSaturday,

                    WeekdayOpen = model.WeekdayOpen,
                    WeekdayClose = model.WeekdayClose,
                    WeekendOpen = model.WeekendOpen,
                    WeekendClose = model.WeekendClose
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

                TempData["Success"] = "Restaurant created successfully";

                return RedirectToAction("Detail", "Restaurant", new { id = restaurant.Id });
            }

            TempData["Error"] = "Restaurant could not be created";
            return View(model);
        }

        // Get edit restaurant form
        // GET: /restaurants/{id}/edit
        // Access: Admin, Business
        [HttpGet("/restaurants/{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.Address)
                .FirstOrDefault(r => r.Id == id);

            // Check if user is logged in
            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to edit a restaurant";
                return RedirectToAction("Login", "Account");
            }

            // Check if user is admin or restaurant owner
            var currentUser = await _userManager.GetUserAsync(User);
            
            _logger.LogInformation("User: " + currentUser.Id);
            _logger.LogInformation("Restaurant: " + restaurant.UserId);
            if ( !User.IsInRole("Admin") && restaurant.UserId != currentUser.Id)
            {
                TempData["Error"] = "You are not authorised to edit this restaurant";
                return RedirectToAction("Detail", "Restaurant", new { id = id });
            }

            if (restaurant == null)
            {
                TempData["Error"] = "Restaurant not found";
                return RedirectToAction("Index", "Restaurant");
            }

            

            var viewModel = new EditRestaurantViewModel
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Email = restaurant.Email,
                PhoneNo = restaurant.PhoneNo,
                Website = restaurant.Website,
                VeganOptions = restaurant.VeganOptions,
                VegetarianOptions = restaurant.VegetarianOptions,
                GlutenFreeOptions = restaurant.GlutenFreeOptions,
                VenueType = restaurant.VenueType,
                Cuisine = restaurant.Cuisine,
                OnlineBooking = restaurant.OnlineBooking,
                DineIn = restaurant.DineIn,
                TakeAway = restaurant.TakeAway,
                Pricing = restaurant.Pricing,
                StreetNumber = restaurant.Address.StreetNumber,
                StreetName = restaurant.Address.StreetName,
                Suburb = restaurant.Address.Suburb,
                State = restaurant.Address.State,
                PostCode = restaurant.Address.PostCode,
                ImageURL = restaurant.ImageURL,

                OpenSunday = restaurant.OpenSunday,
                OpenMonday = restaurant.OpenMonday,
                OpenTuesday = restaurant.OpenTuesday,
                OpenWednesday = restaurant.OpenWednesday,
                OpenThursday = restaurant.OpenThursday,
                OpenFriday = restaurant.OpenFriday,
                OpenSaturday = restaurant.OpenSaturday,

                WeekdayOpen = restaurant.WeekdayOpen,
                WeekdayClose = restaurant.WeekdayClose,
                WeekendOpen = restaurant.WeekendOpen,
                WeekendClose = restaurant.WeekendClose
            };
            

            return View(viewModel);
        }

        // Update restaurant route
        // POST: /restaurants/{id}/edit
        // Access: Admin, Business
        [HttpPost("/restaurants/{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditRestaurantViewModel model)
        {

            var restaurant = _context.Restaurants
                .Include(r => r.Address)
                .FirstOrDefault(r => r.Id == id);

            // Check if user is logged in
            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to edit a restaurant";
                return RedirectToAction("Login", "Account");
            }

            // Check if user is admin or restaurant owner
            var currentUser = await _userManager.GetUserAsync(User);
            if ( !User.IsInRole("Admin") && restaurant.UserId != currentUser.Id)
            {
                TempData["Error"] = "You are not authorised to edit this restaurant";
                return RedirectToAction("Detail", "Restaurant", new { id = id });
            }

            if (restaurant == null)
            {
                TempData["Error"] = "Restaurant not found";
                return RedirectToAction("Index", "Restaurant");
            }

            if (ModelState.IsValid)
            {


                // Restaurant is open on the weekend
                if(model.OpenSaturday || model.OpenSunday) {

                // Check both weekend opening and closing times are not null
                if(model.WeekendOpen == null || model.WeekendClose == null) {
                    TempData["Error"] = "Please enter opening and closing times for the weekend";
                    return View(model);
                }

                // Check opening time is before closing time
                if(model.WeekendOpen >= model.WeekendClose) {
                    TempData["Error"] = "Opening time must be before closing time";
                    return View(model);
                }
                }

            // Restaurant is open on the weekdays
            if(model.OpenMonday || model.OpenTuesday || model.OpenWednesday || model.OpenThursday || model.OpenFriday) {
                    
                    // Check both weekday opening and closing times are not null
                    if(model.WeekdayOpen == null || model.WeekdayClose == null) {
                        TempData["Error"] = "Please enter opening and closing times for the weekdays";
                        return View(model);
                    }
    
                    // Check opening time is before closing time
                    if(model.WeekdayOpen >= model.WeekdayClose) {
                        TempData["Error"] = "Opening time must be before closing time";
                        return View(model);
                    }
            }

            if (!model.OpenSaturday && !model.OpenSunday && !model.OpenMonday && !model.OpenTuesday && !model.OpenWednesday && !model.OpenThursday && !model.OpenFriday) {
                TempData["Error"] = "Please select at least one day of the week";
                return View(model);
            }

                if (restaurant == null)
                {
                    TempData["Error"] = "Restaurant not found";
                    return RedirectToAction("Index", "Restaurant");
                }

                restaurant.Name = model.Name;
                restaurant.Email = model.Email;
                restaurant.PhoneNo = model.PhoneNo;
                restaurant.Website = model.Website;
                restaurant.VenueType = model.VenueType;
                restaurant.Cuisine = model.Cuisine;
                restaurant.Pricing = model.Pricing;
                restaurant.VeganOptions = model.VeganOptions;
                restaurant.VegetarianOptions = model.VegetarianOptions ;
                restaurant.GlutenFreeOptions = model.GlutenFreeOptions ;
                restaurant.OnlineBooking = model.OnlineBooking ;
                restaurant.DineIn = model.DineIn;
                restaurant.TakeAway = model.TakeAway;

                // Update address
                restaurant.Address.StreetNumber = model.StreetNumber;
                restaurant.Address.StreetName = model.StreetName;
                restaurant.Address.Suburb = model.Suburb;
                restaurant.Address.State = model.State;
                restaurant.Address.PostCode = model.PostCode;

                restaurant.OpenMonday = model.OpenMonday;
                restaurant.OpenTuesday = model.OpenTuesday;
                restaurant.OpenWednesday = model.OpenWednesday;
                restaurant.OpenThursday = model.OpenThursday;
                restaurant.OpenFriday = model.OpenFriday;
                restaurant.OpenSaturday = model.OpenSaturday;
                restaurant.OpenSunday = model.OpenSunday;

                restaurant.WeekdayOpen = model.WeekdayOpen;
                restaurant.WeekdayClose = model.WeekdayClose;
                restaurant.WeekendOpen = model.WeekendOpen;
                restaurant.WeekendClose = model.WeekendClose;


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

                _context.Restaurants.Update(restaurant);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Restaurant updated successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // POST: /restaurants/{id}/delete
        [HttpPost("/restaurants/{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            var restaurant = await _context.Restaurants
                .Include(r => r.Address)
                .FirstOrDefaultAsync(r => r.Id == id);

            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to delete a restaurant";
                return RedirectToAction("Login", "Account");
            }

            // Check if user is admin or restaurant owner
            var currentUser = await _userManager.GetUserAsync(User);
             if ( !User.IsInRole("Admin") && restaurant.UserId != currentUser.Id)
            {
                 TempData["Error"] = "You are not authorised to delete this restaurant";
                return RedirectToAction("Detail", "Restaurant", new { id = id });
            }

            if (restaurant == null)
            {
                TempData["Error"] = "Restaurant not found";
                return RedirectToAction("Index", "Restaurant");
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Restaurant deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
