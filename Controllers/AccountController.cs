using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Forage.Models;
using Forage.ViewModels;

using Microsoft.Extensions.Logging;


namespace Forage.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        // private readonly IEmailSender _emailSender;
        // private readonly IUrlHelper _urlHelper;
   
        
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            // _emailSender = emailSender;
            // _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        [HttpGet]
public async Task<IActionResult> ConfirmEmail(string userId, string token)
{
    if (userId == null || token == null)
    {
        return RedirectToAction("Index", "Home");
    }

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        return NotFound($"Unable to load user with ID '{userId}'.");
    }

    var result = await _userManager.ConfirmEmailAsync(user, token);
    if (result.Succeeded)
    {
        return View("EmailConfirmed");
    }

    TempData["Error"] = "Error confirming your email. Please try again.";
    return RedirectToAction("Index", "Home");
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
                    // // Generate email confirmation token
                    // var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // // Create email confirmation link
                    // var emailConfirmationLink = _urlHelper.Action("ConfirmEmail", "Account", new { userId = user.Id, token = emailConfirmationToken }, Request.Scheme);

                    // // Send confirmation email
                    // await _emailSender.SendEmailAsync(model.EmailAddress, "Confirm your email", $"Please confirm your account by clicking this link: <a href='{emailConfirmationLink}'>Confirm Email</a>");

                    // // Redirect the user to a page informing them to check their email
                    // return RedirectToAction("CheckYourEmail");

                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    // return RedirectToAction("Index", "Home");
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
                _logger.LogInformation($"Request method: {Request.Method}");
                _logger.LogInformation($"Request path: {Request.Path}");
                _logger.LogInformation($"Request query string: {Request.QueryString}");
                _logger.LogInformation(ModelState.IsValid.ToString());
                _logger.LogInformation($"User Email: {model.EmailAddress}");
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
