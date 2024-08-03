using BaseProject.WebUI.Helpers;
using BaseProject.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.WebUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddIdentity<AppUser, AppRole>(
                opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequiredLength = 6;
                    opt.User.RequireUniqueEmail = true;
                }).AddEntityFrameworkStores<AppIdentityDbContext>();


            builder.Services.ConfigureApplicationCookie(opt => {
                opt.LoginPath = "/Home/Login";
                }

            );

            // add default admin user and role
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                db.Database.EnsureCreated();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

                await DatabaseCustomHelpers.EnsureRoleExists(roleManager, "Admin");
                await DatabaseCustomHelpers.EnsureRoleExists(roleManager, "User");

                await DatabaseCustomHelpers.EnsureAdminUserExists(userManager, roleManager);
                await DatabaseCustomHelpers.EnsureDefaultUsersExists(userManager);

            }




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
