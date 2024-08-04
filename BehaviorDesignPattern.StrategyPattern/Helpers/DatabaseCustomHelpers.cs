using BaseProject.WebUI.Models;
using Microsoft.AspNetCore.Identity;

namespace BaseProject.WebUI.Helpers
{
    public static class DatabaseCustomHelpers
    {
        public static async Task EnsureRoleExists(RoleManager<AppRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new AppRole { Name = roleName };
                await roleManager.CreateAsync(role);
            }
        }

        public static async Task EnsureAdminUserExists(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            var adminEmail = "admin@hotmail.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new AppUser { UserName = "admin", Email = adminEmail };
                var result = await userManager.CreateAsync(user, "Mypass_1122");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        // default 5 user and add user role 
        public static async Task EnsureDefaultUsersExists(UserManager<AppUser> userManager)
        {
            // add default 5 users
            for (int i = 1; i <= 5; i++)
            {
                var userName = $"user{i}";
                var email = $"{userName}@hotmail.com";
                var appuserr = new AppUser { UserName = userName, Email = email };
                var resultt = await userManager.CreateAsync(appuserr, "Mypass_1122");
                if (resultt.Succeeded)
                {
                    await userManager.AddToRoleAsync(appuserr, "User");
                }

            }
        }
    }
}
