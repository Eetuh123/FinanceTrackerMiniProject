using FinanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using System.Globalization;

namespace FinanceTracker.Controllers
{
    public class TransactionsController : Controller
    {

        public IActionResult TransactionsModal()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult SaveTransaction(Transactions transactions)
        {

            if (!transactions.TrySetCurrentUserId(User))
            {
                return Unauthorized();
            }

            DatabaseManipulator.Save(transactions);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(transactions);
            }

            return RedirectToAction("Index", "Main");
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateField(string id, string fieldName, string newValue)
        {
            if (!ObjectId.TryParse(id, out var objId))
                return Json(new { success = false, error = "Invalid Id" });

            var collection = DatabaseManipulator
                                .database!
                                .GetCollection<Transactions>(nameof(Transactions));

            UpdateDefinition<Transactions>? updateDef = null;
            string? formattedValue = null;

            switch (fieldName)
            {
                case "description":
                    updateDef = Builders<Transactions>
                                .Update
                                .Set(t => t.description, newValue);
                    formattedValue = newValue;
                    break;

                case "amount":
                    if (decimal.TryParse(newValue, out var amt))
                    {
                        updateDef = Builders<Transactions>
                                    .Update
                                    .Set(t => t.amount, amt);
                        formattedValue = amt.ToString("C", CultureInfo.GetCultureInfo("fi-FI"));
                    }
                    break;

                case "date":
                    if (DateTime.TryParse(newValue, out var dt))
                    {
                        updateDef = Builders<Transactions>
                                    .Update
                                    .Set(t => t.date, dt);
                        formattedValue = dt.ToShortDateString();
                    }
                    break;
            }

            if (updateDef == null)
                return Json(new { success = false, error = "Invalid field or value" });

            var result = collection.UpdateOne(
                Builders<Transactions>.Filter.Eq(t => t._id, objId),
                updateDef
            );

            if (result.MatchedCount == 0)
                return Json(new { success = false, error = "Record not found" });

            return Json(new
            {
                success = true,
                formattedValue
            });
        }
        [Authorize]
        [HttpGet("user-transactions")]
        public async Task<IActionResult> GetUserTransactions()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ObjectId.TryParse(idClaim, out var userId))
                return Unauthorized();

            var transactions = await DatabaseManipulator
                                   .GetTransactionsForUserAsync(userId);

            return Ok(transactions);
        }

    }
}
