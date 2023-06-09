using System;
using System.Linq;
using System.Threading.Tasks;
using Forage.Data;
using Forage.Models;
using Forage.ViewModels;
using Forage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Forage.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        private readonly ILogger<BookingController> _logger;


        public BookingController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            EmailService emailService,
            ILogger<BookingController> logger)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        // New booking form for a restaurant
        // GET: /restaurants/restaurantId/bookings/new
        // Private: Basic, Admin
        [HttpGet("/restaurants/{restaurantId}/bookings/new")]
        public async Task<IActionResult> Create(int restaurantId)
        {

            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                TempData["Error"] = "Restaurant not found";
                return RedirectToAction("Index", "Restaurant");
            }

            // Check if user is logged in
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to book a table";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user is admin or restaurant owner
            if (User.IsInRole("Business"))
            {
                TempData["Error"] = "Business accounts cannot book tables";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            var viewModel = new CreateBookingViewModel
            {
                RestaurantId = restaurantId,
                RestaurantName = restaurant.Name,
                Date = DateTime.Now.ToString("dd-MM-yyyy"),
                Restaurant = restaurant
                
            };
            
            return View(viewModel);
        }

        // Create a new booking
        // POST: /restaurants/restaurantId/bookings/new
        // Private: Basic, Admin
        [HttpPost("/restaurants/{restaurantId}/bookings/new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int restaurantId, CreateBookingViewModel viewModel)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            if (restaurant == null)
            {
                TempData["Error"] = "Restaurant not found";
                return RedirectToAction("Index", "Restaurant");
            }

            // Check if user is logged in
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to book a table";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user is admin or restaurant owner
            if (User.IsInRole("Business"))
            {
                TempData["Error"] = "Business accounts cannot book tables";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            // Admin can book multiple tables at the same restaurant
            if(!User.IsInRole("Admin"))
            {
                // Check if user has already booked a table at this restaurant
                var existingBooking = await _context.Bookings
                    .Where(b => b.RestaurantId == restaurantId)
                    .Where(b => b.UserId == currentUser.Id)
                    .FirstOrDefaultAsync();

                if (existingBooking != null)
                {
                    TempData["Error"] = "You already have a booking at this restaurant";
                    return View(viewModel);
                }
            }

            // Convert date and time to DateTime
            var bookingStart = DateTime.Parse($"{viewModel.Date} {viewModel.Time}").ToLocalTime();
            bookingStart = bookingStart.ToUniversalTime();

            // Check if booking time is in the past
            if (bookingStart < DateTime.Now)
            {
                TempData["Error"] = "You cannot book a table in the past";
                return View(viewModel);
            }


            // Check if booking time is within restaurant operating hours
            var parsedTime = TimeOnly.Parse(viewModel.Time);
            var defaultTime = new TimeOnly(0, 0, 0);
            var weekdayOpen = restaurant.WeekdayOpen ?? defaultTime;
            var weekdayClose = restaurant.WeekdayClose ?? defaultTime;
            var weekendOpen = restaurant.WeekendOpen ?? defaultTime;
            var weekendClose = restaurant.WeekendClose ?? defaultTime;

            switch (bookingStart.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    if (!restaurant.OpenSaturday)
                    {
                        TempData["Error"] = "Restaurant is closed on Saturdays";
                        return View(viewModel);
                    }
                    else if (parsedTime < restaurant.WeekendOpen.Value || 
                            parsedTime > restaurant.WeekendClose.Value)
                    {
                        TempData["Error"] = "Booking time is outside of restaurant operating hours on Saturdays";
                        return View(viewModel);
                    }
                    break;
                case DayOfWeek.Sunday:
                    if (!restaurant.OpenSunday)
                    {
                        TempData["Error"] = "Restaurant is closed on Sundays";
                        return View(viewModel);
                    }
                    else if (parsedTime < weekendOpen || parsedTime > weekendClose)
                    {
                        TempData["Error"] = "Booking time is outside of restaurant operating hours on Sundays";
                        return View(viewModel);
                    }
                    break;
                case DayOfWeek.Monday:
                    if(!restaurant.OpenMonday)
                    {
                        TempData["Error"] = "Restaurant is closed on Mondays";
                        return View(viewModel);
                    }
                    else if (parsedTime < weekdayOpen || parsedTime > weekdayClose)
                    {
                        TempData["Error"] = "Booking time is outside of restaurant operating hours on Mondays";
                        return View(viewModel);
                    }
                    break;
                case DayOfWeek.Tuesday:
                    if (!restaurant.OpenTuesday)
                    {
                        TempData["Error"] = "Restaurant is closed on Tuesdays";
                        return View(viewModel);
                    }
                    else if (parsedTime < weekdayOpen || parsedTime > weekdayClose)
                    {
                        TempData["Error"] = "Booking time is outside of restaurant operating hours on Tuesdays";
                        return View(viewModel);
                    }
                    break;
                case DayOfWeek.Wednesday:
                    if (!restaurant.OpenWednesday)
                    {
                        TempData["Error"] = "Restaurant is closed on Wednesdays";
                        return View(viewModel);
                    }
                    else if (parsedTime < weekdayOpen || parsedTime > weekdayClose)
                    {
                        TempData["Error"] = "Booking time is outside of restaurant operating hours on Wednesdays";
                        return View(viewModel);
                    }
                    break;
                case DayOfWeek.Thursday:
                    if (!restaurant.OpenThursday)
                    {
                        TempData["Error"] = "Restaurant is closed on Thursdays";
                        return View(viewModel);
                    }
                    else if (parsedTime < weekdayOpen || parsedTime > weekdayClose)
                    {
                        TempData["Error"] = "Booking time is outside of restaurant operating hours on Thursdays";
                        return View(viewModel);
                    }
                    break;
                case DayOfWeek.Friday:
                    if (!restaurant.OpenFriday)
                    {
                        TempData["Error"] = "Restaurant is closed on Fridays";
                        return View(viewModel);
                    }
                    else if (parsedTime < weekdayOpen || parsedTime > weekdayClose)
                    {
                        TempData["Error"] = "Booking time is outside of restaurant operating hours on Fridays";
                        return View(viewModel);
                    }
                    break;
            }

            // Get all bookings for that 2 hour period
            var bookings = await _context.Bookings
                .Where(b => b.RestaurantId == restaurantId)
                .Where(b => (b.BookingStart >= bookingStart.AddHours(-1) && b.BookingStart < bookingStart) || 
                            (b.BookingStart >= bookingStart && b.BookingStart < bookingStart.AddHours(2)))
                .ToListAsync();


            // Get total number of people booked
            var totalPartySize = bookings.Sum(b => b.PartySize);    

            // Check if capacity is available
            if (totalPartySize + viewModel.PartySize > 20)
            {
                TempData["Error"] = "Sorry, there is only capacity for " + (20 - totalPartySize) + " more people at this time ";
                return View(viewModel);
            }

            var booking = new Booking {
                RestaurantId = restaurantId,
                UserId = currentUser.Id,
                BookingStart = bookingStart,
                PartySize = viewModel.PartySize
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking created successfully";

            return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
        }

        // Send reminder email to all users with a booking for today
        // POST: /restaurants/{restaurantId}/bookings/bulk-reminder
        // Private: Admin, Business
        [HttpPost("/restaurants/{restaurantId}/bookings/bulk-reminder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkReminder(int restaurantId)
        {
            // Check if user is logged in
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to send reminders";
                return RedirectToAction("Index", "Restaurant");
            }

            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user is admin or restaurant owner
            if(currentUser.Id == restaurant.UserId && User.IsInRole("Admin"))
            {
                TempData["Error"] = "You do not have permission to send reminders";
                return RedirectToAction("Index", "Restaurant");
            }

            // Check for bookings for today 
            var bookings = await _context.Bookings
                .Where(b => b.RestaurantId == restaurantId)
                .Where(b => b.BookingStart.Date == DateTime.Now.Date)
                .ToListAsync();

            if (bookings.Count == 0)
            {
                TempData["Error"] = "You have no bookings to send reminders for";
                return RedirectToAction("Index", "Restaurant");
            }

            // Send reminders
            foreach (var booking in bookings)
            {
                var user = await _context.Users.FindAsync(booking.UserId);
                
                var message = $"Hi {user.FirstName}, just a reminder that you have a booking at {restaurant.Name} at {booking.BookingStart.ToString("HH:mm")} for {booking.PartySize} people. See you soon!";

                await _emailService.SendEmailAsync(user.Email, "Booking Reminder", message);
            }

            TempData["Success"] = "Reminders sent";
            return RedirectToAction("Index", "Restaurant");
        }
    }
}
