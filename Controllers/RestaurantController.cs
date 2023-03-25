using Forage.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
    }
}