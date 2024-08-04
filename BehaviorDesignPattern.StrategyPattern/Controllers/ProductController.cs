using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseProject.WebUI.Models;
using BehaviorDesignPattern.StrategyPattern.Models.Entity;
using BehaviorDesignPattern.StrategyPattern.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BehaviorDesignPattern.StrategyPattern.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductRepository _context;
        private readonly UserManager<AppUser> _userManager;
        

        public ProductController(IProductRepository context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            
        }


        // GET: Product
        public async Task<IActionResult> Index()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            
            var repositoryType = _context.GetType().Name;
            
            ViewData["RepositoryType"] = repositoryType;
            
            return View(await _context.GetAllByUserId(user.Id));
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null 
                ||string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _context.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,UserId,CreatedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                await  _context.Add(product);
                return RedirectToAction(nameof(Index));
            }

            product.CreatedDate = DateTime.Now;
            product.UserId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
            await _context.Add(product: product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            await _context.Update(product);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Price,Stock,UserId,CreatedDate")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Update(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _context.GetById(id);
            if (product != null)
            {
                _context.Delete(product);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(string id)
        {
            var product = await _context.GetById(id);
            return product != null;
        }
    }
}
