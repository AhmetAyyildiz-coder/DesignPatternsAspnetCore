using System.Security.Claims;
using BaseProject.WebUI.Helpers;
using BaseProject.WebUI.Models;
using BehaviorDesignPattern.StrategyPattern.Models;
using BehaviorDesignPattern.StrategyPattern.Repositories;
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


            #region Db Context

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            #endregion

            #region Identity

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


            builder.Services.ConfigureApplicationCookie(opt => { opt.LoginPath = "/Home/Login"; }
            );

            #endregion

            #region Default Admin And Users

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

            #endregion


            // bunu direkt bu şekilde kullanmayacağız çünkü strategy pattern kullanacağız 
            // builder.Services.AddScoped<IProductRepository, ProductRepositoryFromSqlServer>();

            /*
             * Strategy Pattern
             * Bu sayede runtime'da hangi repository'nin kullanılacağını belirleyebiliriz. Bunu da database'e sorgu atmadan cookie içerisindeki claim ile
             * yapabiliriz.
             */
            builder.Services.AddScoped<IProductRepository>(
                sp =>
                {
                    var settings = new Settings();
                    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;

                    var claim = httpContext.User.Claims.FirstOrDefault(x => x.Type == Settings.claimDatabaseType);

                    if (claim != null)
                    {
                        settings.DatabaseType = (DatabaseType)int.Parse(claim.Value);
                    }
                    else
                    {
                        settings.DatabaseType = settings.GetDefaultDatabaseType;
                    }

                    var db = sp.GetRequiredService<AppIdentityDbContext>();
                    return settings.DatabaseType switch
                    {
                        DatabaseType.SqlServer => new ProductRepositoryFromSqlServer(db),
                        DatabaseType.MongoDb => new ProductRepositoryFromMongoDb(builder.Configuration),
                        _ => throw new NotImplementedException()
                    };
                });

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