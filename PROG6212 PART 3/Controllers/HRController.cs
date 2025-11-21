
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_PART_3.Models;
using PROG6212_PART_3.Models.ViewModels;
using PROG6212_PART_3.Helpers;

namespace PROG6212_PART_3.Controllers
{
    [SessionAuthorize(Roles = new[] { "HR" })]
    public class HRController : Controller
    {
        private readonly AppDbContext _context;

        public HRController(AppDbContext context)
        {
            _context = context;
        }

        // User Management Dashboard
        public IActionResult ManageUsers()
        {
            var users = _context.Users.OrderBy(u => u.Role).ThenBy(u => u.LastName).ToList();
            return View(users);
        }

        // GET: Create User
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Create User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserViewModel model)
        {
           

           

            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(model);
                }

                // Check if email already exists
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }

                // Map the display role to database role
                string dbRole = MapDisplayRoleToDbRole(model.Role);

                

                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = model.Password,
                    Role = dbRole, 
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    HourlyRate = model.HourlyRate,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                try
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"User {user.FullName} created successfully as {model.Role}! Username: {user.Username}";
                    return RedirectToAction("ManageUsers");
                }
                catch (Exception ex)
                {
                    
                    ModelState.AddModelError("", $"Error creating user: {ex.Message}");
                }
            }

            return View(model);
        }

        // GET: Edit User
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = MapDbRoleToDisplayRole(user.Role), 
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                HourlyRate = user.HourlyRate,
                IsActive = user.IsActive
            };

            return View(model);
        }

        // POST: Edit User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            // Remove password validation for edit
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                // Check if username already exists 
                if (_context.Users.Any(u => u.Username == model.Username && u.UserId != model.UserId))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(model);
                }

                // Check if email already exists 
                if (_context.Users.Any(u => u.Email == model.Email && u.UserId != model.UserId))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }

                user.Username = model.Username;
                user.Role = MapDisplayRoleToDbRole(model.Role); // Map the role
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.HourlyRate = model.HourlyRate;
                user.IsActive = model.IsActive;

                // Update password only if provided
                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.PasswordHash = model.Password;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"User {user.FullName} updated successfully!";
                return RedirectToAction("ManageUsers");
            }

            return View(model);
        }

        // POST: Delete User
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                var currentUserId = HttpContext.Session.GetUserId();
                if (user.UserId == currentUserId)
                {
                    TempData["Error"] = "You cannot delete your own account!";
                    return RedirectToAction("ManageUsers");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"User {user.FullName} deleted successfully!";
            }

            return RedirectToAction("ManageUsers");
        }

        // POST: Toggle User Status
        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"User {user.FullName} status updated to {(user.IsActive ? "Active" : "Inactive")}";
            }

            return RedirectToAction("ManageUsers");
        }

        // HR Dashboard
        public IActionResult HRDashboard()
        {
            var approvedClaims = _context.Claims
                .Include(c => c.User)
                .Where(c => c.Status == "Approved")
                .OrderByDescending(c => c.ManagerApprovedDate ?? c.SubmittedDate)
                .ToList();

            return View(approvedClaims);
        }

        // Generate Invoice
        public IActionResult GenerateInvoice(int claimId)
        {
            var claim = _context.Claims.Include(c => c.User).FirstOrDefault(c => c.ClaimId == claimId);

            if (claim == null || claim.Status != "Approved")
            {
                TempData["Error"] = "Claim not found or not approved";
                return RedirectToAction("HRDashboard");
            }

            var invoice = GenerateInvoiceReport(claim);
            var bytes = Encoding.UTF8.GetBytes(invoice);
            return File(bytes, "text/plain", $"Invoice_{claim.ClaimId}_{DateTime.Now:yyyyMMdd}.txt");
        }

        // Generate Monthly Report
        public IActionResult GenerateMonthlyReport()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var claims = _context.Claims
                .Include(c => c.User)
                .Where(c => c.Status == "Approved"
                    && c.SubmittedDate.Month == currentMonth
                    && c.SubmittedDate.Year == currentYear)
                .ToList();

            var report = GeneratePaymentReport(claims, currentMonth, currentYear);
            var bytes = Encoding.UTF8.GetBytes(report);
            return File(bytes, "text/plain", $"Monthly_Report_{currentYear}_{currentMonth:D2}.txt");
        }

        // Generate Summary Report
        public IActionResult GenerateSummaryReport()
        {
            var approvedClaims = _context.Claims
                .Include(c => c.User)
                .Where(c => c.Status == "Approved")
                .ToList();

            var report = GenerateSummaryReportContent(approvedClaims);
            var bytes = Encoding.UTF8.GetBytes(report);
            return File(bytes, "text/plain", $"Summary_Report_{DateTime.Now:yyyyMMdd}.txt");
        }

        // HELPER METHODS

        // Map display role (from dropdown) to database role
        private string MapDisplayRoleToDbRole(string displayRole)
        {
            // Trim any whitespace
            displayRole = displayRole?.Trim() ?? "";

            var mapping = displayRole switch
            {
                "HR (Super User)" => "HR",
                "Programme Coordinator" => "ProgrammeCoordinator",
                "Academic Manager" => "AcademicManager",
                "Lecturer" => "Lecturer",
                // Also handle if they come in already formatted
                "HR" => "HR",
                "ProgrammeCoordinator" => "ProgrammeCoordinator",
                "AcademicManager" => "AcademicManager",
                _ => displayRole // Return as-is if no mapping found
            };

            System.Diagnostics.Debug.WriteLine($"Mapping '{displayRole}' -> '{mapping}'");
            return mapping;
        }

        // Map database role to display role 
        private string MapDbRoleToDisplayRole(string dbRole)
        {
            return dbRole switch
            {
                "HR" => "HR (Super User)",
                "ProgrammeCoordinator" => "Programme Coordinator",
                "AcademicManager" => "Academic Manager",
                "Lecturer" => "Lecturer",
                _ => dbRole 
            };
        }

        private string GenerateInvoiceReport(Claim claim)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("         LECTURER PAYMENT INVOICE       ");
            sb.AppendLine("========================================");
            sb.AppendLine();
            sb.AppendLine($"Invoice Number: INV-{claim.ClaimId:D6}");
            sb.AppendLine($"Invoice Date: {DateTime.Now:dd/MM/yyyy}");
            sb.AppendLine();
            sb.AppendLine("LECTURER DETAILS:");
            sb.AppendLine($"Name: {claim.User.FullName}");
            sb.AppendLine($"Email: {claim.User.Email}");
            sb.AppendLine();
            sb.AppendLine("CLAIM DETAILS:");
            sb.AppendLine($"Claim ID: {claim.ClaimId}");
            sb.AppendLine($"Hours Worked: {claim.HoursWorked}");
            sb.AppendLine($"Hourly Rate: R{claim.HourlyRate:F2}");
            sb.AppendLine($"Submitted Date: {claim.SubmittedDate:dd/MM/yyyy}");
            sb.AppendLine();
            sb.AppendLine("PAYMENT CALCULATION:");
            sb.AppendLine($"Total Amount: R{claim.TotalAmount:F2}");
            sb.AppendLine();
            sb.AppendLine("========================================");
            sb.AppendLine("Please process payment to the lecturer");
            sb.AppendLine("========================================");

            return sb.ToString();
        }

        private string GeneratePaymentReport(List<Claim> claims, int month, int year)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("      MONTHLY PAYMENT REPORT");
            sb.AppendLine("========================================");
            sb.AppendLine($"Report Period: {month:D2}/{year}");
            sb.AppendLine($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine();
            sb.AppendLine($"Total Claims: {claims.Count}");
            sb.AppendLine($"Total Amount: R{claims.Sum(c => c.TotalAmount):F2}");
            sb.AppendLine();
            sb.AppendLine("BREAKDOWN BY LECTURER:");
            sb.AppendLine("----------------------------------------");

            var lecturerGroups = claims.GroupBy(c => c.User.FullName);

            foreach (var group in lecturerGroups)
            {
                var lecturerTotal = group.Sum(c => c.TotalAmount);
                var lecturerHours = group.Sum(c => c.HoursWorked);

                sb.AppendLine($"Lecturer: {group.Key}");
                sb.AppendLine($"  Claims: {group.Count()}");
                sb.AppendLine($"  Total Hours: {lecturerHours}");
                sb.AppendLine($"  Total Amount: R{lecturerTotal:F2}");
                sb.AppendLine();
            }

            sb.AppendLine("========================================");
            return sb.ToString();
        }

        private string GenerateSummaryReportContent(List<Claim> claims)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("    APPROVED CLAIMS SUMMARY REPORT");
            sb.AppendLine("========================================");
            sb.AppendLine($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine();
            sb.AppendLine("STATISTICS:");
            sb.AppendLine($"Total Approved Claims: {claims.Count}");
            sb.AppendLine($"Total Amount Paid: R{claims.Sum(c => c.TotalAmount):F2}");
            sb.AppendLine($"Total Hours: {claims.Sum(c => c.HoursWorked)}");
            sb.AppendLine($"Average Claim Amount: R{(claims.Any() ? claims.Average(c => c.TotalAmount) : 0):F2}");
            sb.AppendLine();
            sb.AppendLine("DETAILED LIST:");
            sb.AppendLine("----------------------------------------");

            foreach (var claim in claims)
            {
                sb.AppendLine($"Claim ID: {claim.ClaimId}");
                sb.AppendLine($"  Lecturer: {claim.User.FullName}");
                sb.AppendLine($"  Hours: {claim.HoursWorked} | Rate: R{claim.HourlyRate:F2}");
                sb.AppendLine($"  Amount: R{claim.TotalAmount:F2}");
                sb.AppendLine($"  Date: {claim.SubmittedDate:dd/MM/yyyy}");
                sb.AppendLine();
            }

            sb.AppendLine("========================================");
            return sb.ToString();
        }
    }
}