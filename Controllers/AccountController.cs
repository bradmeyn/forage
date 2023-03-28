using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Forage.Models;
using Forage.ViewModels;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Forage.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly EmailService _emailService;
        
   
        public AccountController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            ILogger<AccountController> logger, 
            EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
        }


        // REGISTER ACTIONS

        [HttpGet] // GET: /Account/Register
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost] // POST: /Account/Register
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            

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
                    AccountType = model.AccountType
                };

                // Save new user to database
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Send welcome email with SVG attachment
                    // var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "burger-logo.svg");
                    await _emailService.SendEmailAsync(model.EmailAddress, "Welcome to Forage", "Thank you for creating an account with us!");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // LOGIN ACTIONS
        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View("~/Views/Account/Login.cshtml", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (user != null)
                {
                    _logger.LogInformation("User is not null");
                    _logger.LogInformation(user.FirstName);
                }
                else {
                    System.Diagnostics.Debug.WriteLine($"User is null");
                    }
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
            return View(model);
        } 

        // LOGOUT ACTION
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


    }
}
