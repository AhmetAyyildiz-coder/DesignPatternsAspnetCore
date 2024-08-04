using BehaviorDesignPattern.StrategyPattern.Models.Entity;

namespace BehaviorDesignPattern.StrategyPattern.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetById(string id);

        // get all by user id
        Task<List<Product>> GetAllByUserId(int userId);

        Task Update(Product product);
        Task Delete(Product product);

        Task<Product> Add(Product product);
        
        Task<List<Product>> GetAll();

    }
}
