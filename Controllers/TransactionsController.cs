using FinanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FinanceTracker.Controllers
{
    public class TransactionsController : Controller
    {
        public IActionResult TransactionsModal()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveTransaction(Transactions transactions)
        {

            if (!transactions.TrySetCurrentUserId(User))
            {
                return Unauthorized();
            }

            DatabaseManipulator.Save(transactions);

            return RedirectToAction("Index", "Main");
        }
        [Authorize]
        [HttpGet("user‑transactions")]
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
