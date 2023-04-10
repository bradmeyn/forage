using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Forage.Data;
using Forage.Models;
using Forage.Services;
using Forage.SeedData;
using DotNetEnv;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;


// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Get the connection string for the database from environment variables
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new InvalidOperationException("Database URL not found in environment variables.");

// Get the SendGrid API key from environment variables and store it in the configuration
var sendGridKey = Environment.GetEnvironmentVariable("SENDGRID_KEY") ?? throw new InvalidOperationException("SendGrid key not found in environment variables.");
builder.Configuration["SendGrid:ApiKey"] = sendGridKey;

// Register the ImageUploadService
builder.Services.AddTransient<IImageUploadService, ImageUploadService>();

// Register the EmailService
builder.Services.AddTransient<EmailService>();

builder.Services.AddSingleton(x =>
{
    var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL") ?? throw new InvalidOperationException("CLOUDINARY_URL not found in environment variables.");
    var cloudinary = new CloudinaryDotNet.Cloudinary(cloudinaryUrl);
    cloudinary.Api.Secure = true;
    return cloudinary;
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

// Configure Identity framework with User model and ApplicationDbContext
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login"; 
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/accessdenied";
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed the database with data
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (userManager != null && roleManager != null)
    {
        DefaultRoles.SeedRoles(roleManager, userManager).Wait();
    }
}


app.Run();
