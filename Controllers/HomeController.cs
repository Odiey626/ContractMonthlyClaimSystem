using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        // --- HOME (Landing Page after login) ---
        public IActionResult Home()
        {
            // protect page: redirect to login if not logged in
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }


        // --- CLAIM HISTORY ---
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public IActionResult CoordinatorDashboard()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public IActionResult HRDashboard()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // --- LOGIN ---
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, string role)
        {
            // Demo login logic
            if (username == "test@example.com" && password == "12345")
            {
                // Save session flag
                HttpContext.Session.SetString("UserLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", role);

                // ✅ Redirect to Home Page
                return RedirectToAction("Home");
            }

            ViewBag.Error = "Invalid login details!";
            return View();
        }

        // --- REGISTER ---
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string fullname, string email, string username, string password, string role)
        {
            // In real app: save user to DB
            // For now: redirect to Login after register
            return RedirectToAction("Login");
        }

        // --- LOGOUT ---
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}


