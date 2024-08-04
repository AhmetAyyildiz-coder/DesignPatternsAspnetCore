using BaseProject.WebUI.Models;
using BehaviorDesignPattern.StrategyPattern.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace BehaviorDesignPattern.StrategyPattern.Repositories
{
    public class ProductRepositoryFromSqlServer : IProductRepository
    {
        private readonly AppIdentityDbContext _context;

        public ProductRepositoryFromSqlServer(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<Product> Add(Product product)
        {
            try
            {
                if (product.Id is null)
                {
                    product.Id = Guid.NewGuid().ToString();
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Product>> GetAll()
        {
            try
            {
                return await _context.Products.ToListAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task Delete(Product product)
        {
            try
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public Task<List<Product>> GetAllByUserId(int userId)
        {
            try
            {
                return Task.FromResult(_context.Products.Where(x => x.UserId == userId).ToList());
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Product> GetById(string id)
        {
            try
            {
                return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task Update(Product product)
        {
            try
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}