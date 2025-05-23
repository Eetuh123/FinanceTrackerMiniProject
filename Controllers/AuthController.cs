using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using FinanceTracker.Models;

namespace FinanceTracker.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }


        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Main");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser(string username, string password)
        {
            var user = DatabaseManipulator.database
                .GetCollection<User>("User")
                .Find(u => u.username == username)
                .FirstOrDefault();

            if (user == null)
                return RedirectToAction("Login", "Auth");

            bool isPlainText = !user.password.StartsWith("$2") && !user.password.StartsWith("AQAAAA");
            if (isPlainText && user.password == password)
            {
                user.password = _passwordHasher.HashPassword(user, password);
                DatabaseManipulator.database
                    .GetCollection<User>("User")
                    .ReplaceOne(u => u._id == user._id, user);
            }
            else if (_passwordHasher.VerifyHashedPassword(user, user.password, password) != PasswordVerificationResult.Success)
            {
                return RedirectToAction("Login", "Auth");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, user._id.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Main");
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(User user)
        {
            try
            {
                var collection = DatabaseManipulator.database.GetCollection<User>("User");

                if (collection.Find(u => u.username == user.username).Any())
                {
                    ModelState.AddModelError("username", "Username is already taken.");
                    return View("Register", user);
                }

                user.password = _passwordHasher.HashPassword(user, user.password);
                DatabaseManipulator.Save(user);

                // Sign in immediately after registration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.username),
                    new Claim(ClaimTypes.NameIdentifier, user._id.ToString()),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Main");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Register", user);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
