using Forage.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Forage.Models;
using Forage.ViewModels;


namespace Forage.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/restaurants")]
        public IActionResult Index()
        {
            var restaurants = _context.Restaurants.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Restaurant model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}