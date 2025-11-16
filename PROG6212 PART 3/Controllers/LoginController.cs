using Microsoft.AspNetCore.Mvc;
using PROG6212_PART_3.Models;
using PROG6212_PART_3.Models.ViewModels;


namespace PROG6212_PART_3.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Index()
        {
            // Redirect if already logged in
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var role = HttpContext.Session.GetString("Role");
                return RedirectToDashboard(role);
            }

            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find user by username
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.IsActive);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View(model);
            }

            // Verify password
            bool isPasswordValid = (model.Password == user.PasswordHash);

            if (!isPasswordValid)
            {
                ViewBag.Error = "Invalid username or password";
                return View(model);
            }

            // Update last login
            user.LastLogin = DateTime.Now;
            _context.SaveChanges();

            // Create session
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Email", user.Email);

            TempData["Success"] = $"Welcome back, {user.FullName}!";

            // Redirect based on role
            return RedirectToDashboard(user.Role);
        }

        // Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Login");
        }

        // Access Denied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Helper method to redirect to appropriate dashboard
        private IActionResult RedirectToDashboard(string role)
        {
            return role switch
            {
                "HR" => RedirectToAction("ManageUsers", "HR"),
                "Lecturer" => RedirectToAction("Dashboard", "Lecturer"),
                "ProgrammeCoordinator" => RedirectToAction("ApproveClaim", "ProgrammeCoOrdinator"),
                "AcademicManager" => RedirectToAction("ClaimApproval", "AcademicManager"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
