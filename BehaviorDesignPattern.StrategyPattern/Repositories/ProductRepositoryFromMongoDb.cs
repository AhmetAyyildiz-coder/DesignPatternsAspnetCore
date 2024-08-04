using BehaviorDesignPattern.StrategyPattern.Models.Entity;
using MongoDB.Driver;

namespace BehaviorDesignPattern.StrategyPattern.Repositories;

public class ProductRepositoryFromMongoDb : IProductRepository
{
    
    // mongodb context
    private readonly IMongoCollection<Product> _products;
    private readonly IConfiguration _configuration;

    public ProductRepositoryFromMongoDb(IConfiguration configuration)
    {
        _configuration = configuration;
        try
        {
            var client = new MongoClient(_configuration.GetConnectionString("MongoDbConnection"));
            var database = client.GetDatabase("ProductDb");
            _products = database.GetCollection<Product>("Products");
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async  Task<Product> GetById(string id)
    {
        try
        {
            return await _products.FindAsync(x => x.Id == id).Result.FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Product>> GetAllByUserId(int userId)
    {
        try
        {
            return await _products.FindAsync(x => x.UserId == userId).Result.ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async  Task Update(Product product)
    {
        try
        {
            await _products.ReplaceOneAsync(x => x.Id == product.Id, product);
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task Delete(Product product)
    {
        try
        {
            await _products.DeleteOneAsync(x => x.Id == product.Id);
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Product> Add(Product product)
    {
        try
        {
            await _products.InsertOneAsync(product);
            return product;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Product>> GetAll()
    {
        try
        {
            return await _products.Find(x => true).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}