using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Forage.Models;
using Forage.Services;
using Forage.ViewModels;
using Forage.Data;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;



namespace Forage.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly IImageUploadService _imageUploadService;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

   
        public AccountController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager, 
            EmailService emailService,
            IImageUploadService imageUploadService,
            ILogger<AccountController> logger,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _imageUploadService = imageUploadService;
            _logger = logger;
            _context = context;

        }

        // Sign Up Form
        [HttpGet("/register")]
        public IActionResult Register()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var response = new RegisterViewModel();
            return View(response);
        }

        // POST: /register
        [HttpPost("/register")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
             if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Check if email address is already in use
                var existingUser = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (existingUser != null)
                {
                    TempData["Error"] = "Account already exists for this email address. Please login.";

                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    UserName = model.EmailAddress,
                    Email = model.EmailAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    // Todo: Add address 
                };

                // Upload profile picture to Cloudinary
                if (model.ProfilePicture != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.ProfilePicture.CopyToAsync(memoryStream);
                        user.ProfileURL = await _imageUploadService.UploadImageAsync(memoryStream.ToArray(), model.ProfilePicture.FileName);
                    }
                }
                else {
                    user.ProfileURL = "https://res.cloudinary.com/dzkirdhwv/image/upload/v1680477347/pexels-andra-918581_z4mlyj.jpg";
                }

                // Save new user to database
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign role based on account type 
                    switch (model.AccountType)
                    {
                        case "Foodie":
                            await _userManager.AddToRoleAsync(user, "Basic");
                            break;
                        case "Business":
                            await _userManager.AddToRoleAsync(user, "Business");
                            break;
                        default:
                            await _userManager.AddToRoleAsync(user, "Basic");
                            break;
                    }

                    // Send welcome email
                    await _emailService.SendEmailAsync(model.EmailAddress, "Welcome to Forage", "Thank you for creating a " + model.AccountType + " an account with Forage. We hope you enjoy your experience!");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }

        // Login Form
        [HttpGet("/login")]
        public IActionResult Login(string returnUrl = null)
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var response = new LoginViewModel { ReturnUrl = returnUrl };
            return View("~/Views/Account/Login.cshtml", response);
        }

        // Google External Login
        [HttpGet("/google-login")]
        public async Task<IActionResult> GoogleLogin()
        {
            // Configure the redirect URL for Google authentication
            // var redirectUrl = Url.Action("GoogleLoginCallback", "Account");
            var redirectUrl = "https://127.0.0.1:5173/google-login-callback";

            // Configure the authentication properties for Google authentication
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            // Redirect the user to the Google authentication page
            return new ChallengeResult("Google", properties);
        }

        [HttpGet("/google-login-callback")]
        public async Task<IActionResult> GoogleLoginCallback( string remoteError = null)
        {
            var returnUrl = Url.Action("Index", "Restaurant");

            if (remoteError != null)
            {
                TempData["Error"] = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login", "Account");
            }

            // Get the external login info from the Google provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["Error"] = "Error loading external login information.";
                return RedirectToAction("Login", "Account");
            }

            // Attempt to sign in the user using the external login info
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            // If the user does not have an account, create a new account and sign in
            if (result.IsLockedOut)
            {
                return RedirectToPage("/Account/Lockout");
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                    ProfileURL = info.Principal.FindFirstValue("urn:google:picture"),
                    // Add additional fields from the external login info as needed
                };

                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    createResult = await _userManager.AddLoginAsync(user, info);
                    if (createResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Login", new LoginViewModel { ReturnUrl = returnUrl });
            }
        }

        // POST: /login
        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                _logger.LogInformation("ModelState is valid.");
                var result = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                     TempData["Error"] = "Invalid credentials. Please try again.";
                }
            }
            else {
                TempData["Error"] = "Invalid credentials. Please try again.";
            }
            return View(model);
        } 

        // LOGOUT ACTION
        // POST: /logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Restaurant");
        }

        // ADMIN HUB
        [HttpGet("/admin")]
        public IActionResult Admin()
        {
            if(!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You must be logged in to access the this page.";
                return RedirectToAction("Login", "Account");
            }

            var users = _context.Users.ToList();
            var restaurants = _context.Restaurants.Include(r => r.User).Include(r => r.Reviews).ToList();
            var reviews = _context.Reviews.Include(r => r.Restaurant).Include(r => r.User).ToList();
            
            var model = new AdminViewModel
            {
                Users = users,
                Restaurants = restaurants,
                Reviews = reviews
            };

            return View(model);
        }

       [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to access this page.";
                return RedirectToAction("Login", "Account");
            }

            if (!User.IsInRole("Business"))
            {
                TempData["Error"] = "Only business accounts can access the business dashboard.";
                return RedirectToAction("Index", "Restaurant");
            }

            var userId = _userManager.GetUserId(User);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var restaurant = _context.Restaurants
                                    .Include(r => r.Address)
                                    .Include(r => r.Reviews)
                                    .ThenInclude(r => r.User)
                                    .Include(r => r.Bookings)
                                    .ThenInclude(r => r.User)
                                    .FirstOrDefault(r => r.UserId == userId);

            var today = DateTime.Now.Date;
            var bookingsToday = restaurant.Bookings.Where(b => b.BookingStart.Date == today).ToList();
            


            _logger.LogInformation("Bookings today: " + bookingsToday.Count);
            _logger.LogInformation("Date " + DateTime.Now.Date);

            var allBookings = _context.Bookings.ToList();

            _logger.LogInformation("All bookings: " + allBookings.Count);

            foreach (var booking in allBookings)
            {
                _logger.LogInformation("Booking date: " + booking.BookingStart);
                _logger.LogInformation("Booking Restaurant " + booking.RestaurantId);
                _logger.LogInformation("restaurant id" + restaurant.Id);
            }

            var recentReviews = restaurant.Reviews.OrderByDescending(r => r.CreatedAt).Take(5).ToList();

            var fiveStarReviews = restaurant.Reviews.Where(r => r.Rating == 5).Count();
            var fourStarReviews = restaurant.Reviews.Where(r => r.Rating == 4).Count();
            var threeStarReviews = restaurant.Reviews.Where(r => r.Rating == 3).Count();
            var twoStarReviews = restaurant.Reviews.Where(r => r.Rating == 2).Count();
            var oneStarReviews = restaurant.Reviews.Where(r => r.Rating == 1).Count();

            var model = new DashboardViewModel
            {
                User = user,
                Restaurant = restaurant,
                ReviewCount = restaurant.Reviews.Count > 0 ? restaurant.Reviews.Count : 0,
                BookingCount = restaurant.Bookings.Count > 0 ? restaurant.Bookings.Count : 0,
                AverageRating = restaurant.Reviews.Count > 0 ? restaurant.Reviews.Average(r => r.Rating) : 0,
                BookingsToday = bookingsToday,
                RecentReviews = recentReviews,

                FiveStarReviews = fiveStarReviews,
                FourStarReviews = fourStarReviews,
                ThreeStarReviews = threeStarReviews,
                TwoStarReviews = twoStarReviews,
                OneStarReviews = oneStarReviews
            };

            return View(model);
        }



        //Bulk Email Offer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkOffer(int restaurantId, string subject, string message)
        {
            _logger.LogInformation("Bulk Offer");
                        _logger.LogInformation("Restaurant Id: " + restaurantId);
            _logger.LogInformation("Message: " + message);

            _logger.LogInformation("Subject: " + subject);
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User is not authenticated.");
                TempData["Error"] = "You must be logged in to access this page.";
                return RedirectToAction("Login", "Account");
            }

            if (User.IsInRole("Basic"))
            {
                _logger.LogInformation("User is not a business or admin.");
                TempData["Error"] = "Foodie accounts cannot access this feature.";
                return RedirectToAction("Index", "Restaurant");
            }

            // Get all users with account role of Basic (Foodie)
            var basicUsers = await _userManager.GetUsersInRoleAsync("Basic");


            _logger.LogInformation("Basic Users: " + basicUsers.Count);

            

            var userEmails = new List<string>();

            foreach (var user in basicUsers)
            {
                userEmails.Add(user.Email);
            }

            await _emailService.SendBulkEmailAsync(userEmails, subject, message);

            TempData["Success"] = "Emails sent successfully.";
            return RedirectToAction("Dashboard", "Account");
        }

    }
}
