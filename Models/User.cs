using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using static FinanceTracker.Models.DatabaseManipulator;

namespace FinanceTracker.Models
{
    public class User : IMongoDocument
    {
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
        public string username { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters")]
        public string name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6)]
        public string password { get; set; }

        [Range(0, 1000000, ErrorMessage = "Savings goal must be a positive number")]
        public decimal? SavingsGoal { get; set; }

    }
}
