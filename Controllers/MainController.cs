using System.Diagnostics;
using System.Security.Claims;
using FinanceTracker.Models;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Globalization;
using MongoDB.Driver;

namespace FinanceTracker.Controllers
{
    public class MainController : Controller
    {
        private readonly ILogger<MainController> _logger;

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ObjectId.TryParse(idClaim, out var userId))
                return Unauthorized();

            var currentUser = await DatabaseManipulator.GetUserByIdAsync(userId);
            decimal savingsGoal = currentUser?.SavingsGoal ?? 500m;

            var txs = await DatabaseManipulator.GetTransactionsForUserAsync(userId);

            var grouped = txs
                .GroupBy(t => t.date.ToString("MMM yyyy"))
                .OrderBy(g => DateTime.ParseExact(g.Key, "MMM yyyy", CultureInfo.InvariantCulture));
            var labels = grouped.Select(g => g.Key).ToArray();
            var income = grouped.Select(g => g.Where(t => t.amount > 0).Sum(t => t.amount)).ToArray();
            var expense = grouped.Select(g => -g.Where(t => t.amount < 0).Sum(t => t.amount)).ToArray();

            var today = DateTime.Today;
            var periodStart = new DateTime(today.Year, today.Month, 1);

            var nextIncomeTx = txs
                .Where(t => t.amount > 0 && t.date.Date > today)
                .OrderBy(t => t.date)
                .FirstOrDefault();

            DateTime nextPayday = nextIncomeTx != null
                ? nextIncomeTx.date.Date
                : today;

            int daysUntilNextPayday = (nextPayday - today).Days;

            var thisMonthTx = txs
                .Where(t => t.date.Date >= periodStart && t.date.Date <= today)
                .ToList();

            var incomeThisPeriod = thisMonthTx.Where(t => t.amount > 0).Sum(t => t.amount);
            var expensesThisPeriod = thisMonthTx.Where(t => t.amount < 0).Sum(t => Math.Abs(t.amount));

            int daysElapsed = (today - periodStart).Days + 1;
            decimal dailyExpenseRate = daysElapsed > 0
                ? expensesThisPeriod / daysElapsed
                : 0m;

            decimal potentialSavingsLeft = (incomeThisPeriod - savingsGoal) - expensesThisPeriod;

            var vm = new HomeViewModel
            {
                UserName = User.Identity.Name!,
                Transactions = txs,
                ChartLabels = labels,
                IncomeSeries = income,
                ExpenseSeries = expense,
                NextPayday = nextPayday,
                DaysUntilNextPayday = daysUntilNextPayday,
                IncomeThisPeriod = incomeThisPeriod,
                ExpensesThisPeriod = expensesThisPeriod,
                DailyExpenseRate = dailyExpenseRate,
                SavingsGoal = savingsGoal,
                PotentialSavings = potentialSavingsLeft
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> SetSavingsGoal(decimal savingsGoal)
        {

            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ObjectId.TryParse(idClaim, out var userId))
                return Unauthorized();

            var usersColl = DatabaseManipulator
                .database!
                .GetCollection<User>(nameof(User));

            var filter = Builders<User>.Filter.Eq(u => u._id, userId);
            var update = Builders<User>.Update.Set(u => u.SavingsGoal, savingsGoal);
            await usersColl.UpdateOneAsync(filter, update);

            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
