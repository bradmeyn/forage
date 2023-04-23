using Forage.Data;
using Forage.Models;
using Forage.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace Forage.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(ApplicationDbContext context, UserManager<User> userManager, ILogger<ReviewController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // New review form
        // GET: /restaurants/{restaurantId}/reviews/new
        // Private: Basic, Admin
        [HttpGet("/restaurants/{restaurantId}/reviews/new")]
        public async Task<IActionResult> Create(int restaurantId)
        {
            // Check if user is logged in
            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to leave a review";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            // Check if user is admin or restaurant owner
            if (User.IsInRole("Business"))
            {
                TempData["Error"] = "Business accounts cannot leave reviews";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            // Check if user has already left a review for this restaurant
            var currentUser = await _userManager.GetUserAsync(User);
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.RestaurantId == restaurantId && r.UserId == currentUser.Id);
            if (review != null && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You have already left a review for this restaurant";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            var viewModel = new CreateReviewViewModel
            {
                RestaurantId = restaurantId,
                RestaurantName = _context.Restaurants.FirstOrDefault(r => r.Id == restaurantId).Name
            };

            return View(viewModel);
        }

        // Create a new review
        // POST: /restaurants/{restaurantId}/reviews/new
        // Private: Basic, Admin
        [HttpPost("/restaurants/{restaurantId}/reviews/new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewViewModel model, int restaurantId )
        {
             // Check if user is logged in
            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to leave a review";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            // Check if user is business
            if (User.IsInRole("Business"))
            {
                TempData["Error"] = "Business accounts cannot leave reviews";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            // Check if user has already left a review for this restaurant
            var currentUser = await _userManager.GetUserAsync(User);
            var existingReview = await _context.Reviews.FirstOrDefaultAsync(r => r.RestaurantId == restaurantId && r.UserId == currentUser.Id);
            
            if (existingReview != null && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You have already left a review for this restaurant";
                return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
            }

            if (ModelState.IsValid)
            {
                var review = new Review
                {
                    RestaurantId = model.RestaurantId,
                    UserId = currentUser.Id,
                    Title = model.Title,
                    Details = model.Details,
                    Rating = model.Rating
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Review added successfully";

                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }
            else 
            {
                TempData["Error"] = "Please correct the errors belo";
                return View(model);
            }

        }

        // Edit review form
        // GET: /reviews/{id}/edit
        // Private: Basic, Admin
        [HttpGet("/reviews/{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);
            
            if (review == null)
            {
                TempData["Error"] = "Review not found";
                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }

            // Check if user is logged in
            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Please login to edit reviews";
                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user is admin or review owner
            if ( !User.IsInRole("Admin") && currentUser.Id != review.UserId)
            {
                TempData["Error"] = "You do not have permission to edit this review";
                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }

            var viewModel = new EditReviewViewModel
            {
                ReviewId = review.ReviewId,
                RestaurantId = review.RestaurantId,
                RestaurantName = _context.Restaurants.FirstOrDefault(r => r.Id == review.RestaurantId).Name,
                UserId = review.UserId,
                Title = review.Title,
                Details = review.Details,
                Rating = review.Rating,
            };


            return View(viewModel);
        }

        // Update a review 
        // POST: /reviews/{id}
        // Private: Basic, Admin
        [HttpPost("/reviews/{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditReviewViewModel model)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);
            if (review == null)
            {
                TempData["Error"] = "Review not found";
                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }

            // Check if user is logged in
            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Please login to edit reviews";
                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user is admin or review owner
            if ( !User.IsInRole("Admin") && currentUser.Id != review.UserId)
            {
                TempData["Error"] = "You do not have permission to edit this review";
                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }

            if (ModelState.IsValid)
            {
                review.Title = model.Title;
                review.Details = model.Details;
                review.Rating = model.Rating;
                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Review updated successfully";

                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }
            TempData["Error"] = "There was an error updating your review";
            return View(model);
        }

        // Delete a review
        // POST: /reviews/{id}/delete
        // Private: Basic, Admin
        [HttpPost("/reviews/{id}/delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null){
                TempData["Error"] = "Review not found";
                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }

             // Check if user is logged in
            if(!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Please login to delete reviews";
                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }

            // Check if user is admin or review owner
            if ( !User.IsInRole("Admin") && currentUser.Id != review.UserId)
            {
                TempData["Error"] = "You do not have permission to edit this review";
                return RedirectToAction("Detail", "Restaurant", new { id = review.RestaurantId });
            }

            var restaurantId = review.RestaurantId;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review deleted successfully";

            return RedirectToAction("Detail", "Restaurant", new { id = restaurantId });
        }
    }
}
