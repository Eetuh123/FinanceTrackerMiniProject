using FinanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;

namespace FinanceTracker.Controllers
{
    public class MoneyController : Controller
    {
        public IActionResult MoneyModal()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveTransaction(Transactions transactions)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = ObjectId.Parse(userIdString);

            DatabaseManipulator.Save(transactions);

            return RedirectToAction("Index", "Main");
        }
    }
}
