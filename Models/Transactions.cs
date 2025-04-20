using MongoDB.Bson;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using static FinanceTracker.Models.DatabaseManipulator;

namespace FinanceTracker.Models
{
    public class Transactions : IMongoDocument
    {
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description can't exceed 100 characters")]
        public string description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string category { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(-100000, 100000, ErrorMessage = "Amount must be between -100,000 and 100,000")]
        public int amount { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; } = ObjectId.Empty;

        public bool TrySetCurrentUserId(ClaimsPrincipal user)
        {
            var userIdString = user.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (ObjectId.TryParse(userIdString, out var id))
            {
                UserId = id;
                return true;
            }
            return false;
        }
    }
    public enum Category
    {
        Food,
        Bills,
        Entertainment,
        Travel,
        Health,
        Income,
    }
}
