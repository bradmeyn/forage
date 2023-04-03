using Forage.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Forage.SeedData
{

    public class DefaultRoles
    {
        private static async Task CreateUserAsync(
            UserManager<User> userManager, 
            string email, string firstName, 
            string lastName, string role, 
            string password, string profileUrl
        )
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    ProfileURL = profileUrl
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Assign the role to the user
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
        

        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {   
            // Seed the roles
            foreach (var roleName in new[] {"Admin", "Basic", "Business" })
            {
                // Check if the role exists
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                   // Create the role
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed the admin user
            await CreateUserAsync(userManager, "admin@forage.com", "Admin", "User", "Admin", "ForageAdmin123!", "https://res.cloudinary.com/dzkirdhwv/image/upload/v1680225868/charlie_profile_mrf4yl.webp");

            // Seed the basic user
            await CreateUserAsync(userManager, "basic@forage.com", "Basic", "User", "Basic", "ForageBasic123!", "https://res.cloudinary.com/dzkirdhwv/image/upload/v1680477115/emily_profile_lp8sej.webp");

            // Seed the business user
            await CreateUserAsync(userManager, "business@forage.com", "Business", "User", "Business", "ForageBusiness123!", "https://res.cloudinary.com/dzkirdhwv/image/upload/v1680212666/fw1n7g6bzordoy5rjb4h.webp");
        }
    }
}
