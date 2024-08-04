using MongoDB.Driver;
namespace BehaviorDesignPattern.StrategyPattern.Models
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetSection("MongoDbSettings:ConnectionString").Value);
            _database = client.GetDatabase(configuration.GetSection("MongoDbSettings:DatabaseName").Value);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }

}
