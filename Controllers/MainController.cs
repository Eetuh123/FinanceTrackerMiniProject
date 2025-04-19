using System.Diagnostics;
using System.Security.Claims;
using FinanceTracker.Models;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers
{
    public class MainController : Controller
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ObjectId.TryParse(idClaim, out var userId))
                return Unauthorized();

            var txs = await DatabaseManipulator.GetTransactionsForUserAsync(userId);
            var vm = new HomeViewModel
            {
                UserName = User.Identity.Name!,
                Transactions = txs
            };
            return View(vm);
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Main");
            }
            return View();
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
