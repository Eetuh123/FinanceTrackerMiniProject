using FinanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;

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
    }
}
