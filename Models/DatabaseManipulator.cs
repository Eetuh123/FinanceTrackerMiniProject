using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Models
{
    public class DatabaseManipulator
    {
        private static IConfiguration? config;
        private static string? DATABASE_NAME;
        private static string? HOST;
        private static MongoServerAddress? address;
        private static MongoClientSettings? settings;
        private static MongoClient? client;
        public static IMongoDatabase? database;
        private static object logger;

        public static void Initialize(IConfiguration configuration)
        {
            config = configuration;
            var sections = config.GetSection("ConnectionStrings");
            DATABASE_NAME = sections.GetValue<string>("DatabaseName");
            HOST = sections.GetValue<string>("MongoConnection");
            address = new MongoServerAddress(HOST);
            settings = new MongoClientSettings() { Server = address };
            client = new MongoClient(settings);
            database = client.GetDatabase(DATABASE_NAME);
        }
        public static T Save<T>(T record) where T : IMongoDocument
        {

            var collection = database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", record._id);

            collection.ReplaceOne(filter, record, new ReplaceOptions { IsUpsert = true });
            return record;

        }
        public static Task<List<Transactions>> GetTransactionsForUserAsync(ObjectId userId)
        {
            return database!
                .GetCollection<Transactions>(nameof(Transactions))
                .Find(t => t.UserId == userId)
                .ToListAsync();
        }
        public static T Delete<T>(T record) where T : IMongoDocument
        {
            var collection = database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", record._id);
            collection.DeleteOne(filter);
            return record;
        }
        public interface IMongoDocument
        {
            ObjectId _id { get; set; }
        }
    }
}
