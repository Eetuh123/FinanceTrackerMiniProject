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
        [Required(ErrorMessage = "Category is required")]
        public string category { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        public int amount { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; } = ObjectId.Empty;

    }
    public enum Category
    {
        Food,
        Bills,
        Entertainment,
        Travel,
        Health
    }
}
