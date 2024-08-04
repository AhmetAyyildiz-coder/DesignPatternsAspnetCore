using System.Security.Claims;
using BaseProject.WebUI.Models;
using BehaviorDesignPattern.StrategyPattern.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BehaviorDesignPattern.StrategyPattern.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SettingsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: SettingsController
        public ActionResult Index()
        {
            Settings settings = new();
            if (User.Claims.Where(x=>x.Type== Settings.claimDatabaseType).FirstOrDefault()!= null)
            {
                settings.DatabaseType = (DatabaseType)int.Parse(User.Claims.Where(x => x.Type == Settings.claimDatabaseType).FirstOrDefault().Value);
            }
            else
            {
                settings.DatabaseType = settings.GetDefaultDatabaseType;
            }
            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeDatabase(Settings model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            var newClaim = new Claim(Settings.claimDatabaseType, ((int)model.DatabaseType).ToString());
            
            var oldClaim = User.Claims.Where(x => x.Type == Settings.claimDatabaseType).FirstOrDefault();
            
            if (oldClaim != null)
            {
                await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
            }
            else
            {
                await _userManager.AddClaimAsync(user, newClaim);
            }
            
            await _signInManager.SignOutAsync();


            var result =  await HttpContext.AuthenticateAsync();
            
            await _signInManager.SignInAsync(user, result.Properties);
            
            return RedirectToAction("Index");
        }
    }
}
