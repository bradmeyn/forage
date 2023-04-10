using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Forage.Models;
using Forage.Services;
using Forage.ViewModels;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;


namespace Forage.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly IImageUploadService _imageUploadService;
   
        public AccountController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            EmailService emailService,
            IImageUploadService imageUploadService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _imageUploadService = imageUploadService;
        }

        // REGISTER ACTIONS
        // GET: /register
        [HttpGet("/register")]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        // POST: /register
        [HttpPost("/register")] 
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

        // LOGIN ACTIONS
        // GET: /login
        [HttpGet("/login")]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View("~/Views/Account/Login.cshtml", response);
        }

        // POST: /login
        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
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
        // POST: /logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Restaurant");
        }
    }
}
