using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Forage.Models;

namespace Forage.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Register
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Register
        // [HttpPost("Register")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Register(User user)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         user.UserName = user.Email;
        //         var result = await _userManager.CreateAsync(user, user.Password);
        //         if (result.Succeeded)
        //         {
        //             await _signInManager.SignInAsync(user, isPersistent: false);
        //             return RedirectToAction("Index", "Home");
        //         }
        //         foreach (var error in result.Errors)
        //         {
        //             ModelState.AddModelError(string.Empty, error.Description);
        //         }
        //     }
        //     return View(user);
        // }

        // GET: /Login
        [HttpGet("Login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Login
        // [HttpPost("Login")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Login(User user, string returnUrl = null)
        // {
        //     ViewData["ReturnUrl"] = returnUrl;
        //     if (ModelState.IsValid)
        //     {
        //         var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, lockoutOnFailure: false);
        //         if (result.Succeeded)
        //         {
        //             return RedirectToLocal(returnUrl);
        //         }
        //         ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //     }
        //     return View(user);
        // }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
