using MongoDB.Driver;
using RoleBaseApi.Models;

namespace RoleBaseApi.Data
{
    // MongoDB database connection class
    public class MongoContext
    {
        private readonly IMongoDatabase _db;

        public MongoContext(IConfiguration config)
        {
            // Connect to MongoDB using connection string from appsettings.json
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            _db = client.GetDatabase("RoleBasedDb"); 
        }

        // Expose Users collection
        public IMongoCollection<User> Users => _db.GetCollection<User>("Users");
        public IMongoCollection<Business> Businesses => _db.GetCollection<Business>("Businesses");
    }
}
