using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Forage.Models;

namespace Forage.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // [HttpGet]
        // public IActionResult Login()
        // {
        //     var response = new LoginViewModel();
        //     return View(response);
        // }

        // [HttpGet]
        // public IActionResult Register()
        // {
        //     var response = new RegisterViewModel();
        //     return View(response);
        // }

    }
}