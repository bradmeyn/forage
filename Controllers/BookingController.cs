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

namespace Forage.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;


        public BookingsController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: /restaurants/restaurantId/bookings/new
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
                
            };

            return View(viewModel);
        }

        // POST: /restaurants/restaurantId/bookings/new
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

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Check if the user already has a booking for this availability
            // ...
            // Save booking and redirect to the appropriate action
            // ...
            return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
        }

        [HttpPost("/restaurants/{restaurantId}/bookings/bulk-reminder")]
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
            if(currentUser.Id == restaurant.UserId || User.IsInRole("Admin"))
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
