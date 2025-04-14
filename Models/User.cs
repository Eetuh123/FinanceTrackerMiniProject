using MongoDB.Bson;

namespace FinanceTracker.Models
{
    public class User
    {
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public string? name { get; set; }
        public string? password { get; set; }

    }
}
