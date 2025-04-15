using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using static FinanceTracker.Models.DatabaseManipulator;

namespace FinanceTracker.Models
{
    public class Transactions : IMongoDocument
    {
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        [Required(ErrorMessage = "Username is required")]
        public string description { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string gategory { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string amount { get; set; }
        public ObjectId UserId { get; set; } = ObjectId.Empty;

    }
}
