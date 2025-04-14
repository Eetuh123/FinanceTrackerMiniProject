using MongoDB.Driver;
using MongoDB.Bson;

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
        public static T Save<T>(T record)
        {
            try
            {
                var collectionName = typeof(T).Name;
                var collection = database.GetCollection<T>(collectionName);

                var idOfThing = typeof(T).GetProperty("_id");

                if (idOfThing == null)
                {
                    throw new Exception("No _Id found called" + typeof(T).Name);
                }

                var idValue = idOfThing.GetValue(record);
                var filter = Builders<T>.Filter.Eq("_id", BsonValue.Create(idValue));

                collection.ReplaceOne(
                    filter,
                    record,
                    new ReplaceOptions { IsUpsert = true }
                );
            }
            catch
            {
                Console.WriteLine("Something aint working");
            }

            return record;
        }
    }
}
