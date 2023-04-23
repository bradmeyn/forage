using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Forage.Data;
using Forage.Models;
using Forage.Services;
using Forage.SeedData;
using DotNetEnv;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;




// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Get the connection string for the database from environment variables
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new InvalidOperationException("Database URL not found in environment variables.");

// Get the SendGrid API key from environment variables and store it in the configuration
var sendGridKey = Environment.GetEnvironmentVariable("SENDGRID_KEY") ?? throw new InvalidOperationException("SendGrid key not found in environment variables.");
builder.Configuration["SendGrid:ApiKey"] = sendGridKey;

var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? throw new InvalidOperationException("Google Client ID not found in environment variables.");
var googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? throw new InvalidOperationException("Google Client Secret not found in environment variables.");
builder.Configuration["Authentication:Google:ClientId"] = googleClientId;
builder.Configuration["Authentication:Google:ClientSecret"] = googleClientSecret;


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

// Configure Google authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        options.Scope.Add("profile");

        options.Events.OnCreatingTicket = async context =>
    {
        var pictureClaim = context.User.GetProperty("picture");
        if (pictureClaim.ValueKind != JsonValueKind.Null)
        {
            var pictureUrl = pictureClaim.GetString();
            context.Identity.AddClaim(new Claim("picture", pictureUrl));
        }
    };        


    });

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

builder.Services.AddCookiePolicy(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
});


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



app.UseStaticFiles();

app.UseRouting(); 
app.UseAuthentication();
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
