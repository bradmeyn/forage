using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Forage.Models;
using Forage.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Forage.Data;

namespace Forage.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context )
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var topRatedRestaurants = _context.Restaurants
                .Include(r => r.Reviews)
                .Select(r => new
                {
                    Restaurant = r,
                    AverageRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0
                })
                .OrderByDescending(r => r.AverageRating)
                .Take(3)
                .Select(r => r.Restaurant)
                .ToList();

        var newestRestaurants = _context.Restaurants.Include(r => r.Reviews)
            .OrderByDescending(r => r.CreatedAt)
            .Take(3)
            .ToList();

        var viewModel = new HomeViewModel
        {
            TopRatedRestaurants = topRatedRestaurants,
            NewestRestaurants = newestRestaurants
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

