using Microsoft.AspNetCore.Identity;
using Forage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Forage.Data;
using DotNetEnv;

// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Get the connection string for the database from environment variables
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new InvalidOperationException("Database URL not found in environment variables.");

// Get the SendGrid API key from environment variables and store it in the configuration
var sendGridKey = Environment.GetEnvironmentVariable("SENDGRID_KEY") ?? throw new InvalidOperationException("SendGrid key not found in environment variables.");
builder.Configuration["SendGrid:ApiKey"] = sendGridKey;

// Add and configure the DbContext (ApplicationDbContext) for the app using the connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add a filter to handle database-related exceptions
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add a singleton service for the ActionContextAccessor to get the current ActionContext
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

// Add and configure the Identity framework with User model and ApplicationDbContext
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add support for controllers with views
builder.Services.AddControllersWithViews();

// Register the EmailService
builder.Services.AddTransient<EmailService>();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use the migration endpoint in development mode
    app.UseMigrationsEndPoint();
}
else
{
    // Use a custom error handler for production
    app.UseExceptionHandler("/Home/Error");
    
    // Use HSTS (HTTP Strict Transport Security) for secure connections
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Serve static files, such as images, CSS, and JavaScript
app.UseStaticFiles();

// Enable routing middleware
app.UseRouting();

// Enable authorization middleware
app.UseAuthorization();

// Define the default route pattern for the application
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Enable Razor Pages
app.MapRazorPages();

// Run the application
app.Run();
