using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_PART_3.Models;
using PROG6212_PART_3.Helpers;

namespace PROG6212_PART_3.Controllers
{
    [SessionAuthorize(Roles = new[] { "Lecturer" })]
    public class LecturerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LecturerController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetUserId();
            var claims = _context.Claims
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            return View(claims);
        }

        public IActionResult SubmitClaim()
        {
            // Get current user info to display
            var userId = HttpContext.Session.GetUserId();
            var user = _context.Users.Find(userId);

            ViewBag.LecturerName = user?.FullName;
            ViewBag.HourlyRate = user?.HourlyRate;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile? document)
        {
            // Get current user
            var userId = HttpContext.Session.GetUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                TempData["Error"] = "User not found. Please log in again.";
                return RedirectToAction("Index", "Login");
            }

            // Auto-populate user information
            claim.UserId = user.UserId;

            // Automated validation checks
            var validationErrors = new List<string>();

            // Check hours worked - MUST be between 0.1 and 180 (monthly limit)
            if (claim.HoursWorked < 0.1 || claim.HoursWorked > 180)
            {
                validationErrors.Add("Hours worked must be between 0.1 and 180 (monthly limit)");
                claim.IsHoursValid = false;
            }
            else
            {
                claim.IsHoursValid = true;
            }

            // Auto-calculate total amount using user's hourly rate
            var totalAmount = claim.HoursWorked * user.HourlyRate;

            // Check if total amount is reasonable
            if (totalAmount > 50000)
            {
                validationErrors.Add("Total claim amount exceeds maximum limit of R50,000");
                claim.IsAmountValid = false;
            }
            else
            {
                claim.IsAmountValid = true;
            }

            if (validationErrors.Any())
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError("", error);
                }
                ViewBag.LecturerName = user.FullName;
                ViewBag.HourlyRate = user.HourlyRate;
                return View(claim);
            }

            // Remove validation for fields that are auto-populated
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("LecturerName");

            if (ModelState.IsValid)
            {
                // Automated file validation
                if (document != null && document.Length > 0)
                {
                    if (document.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "File size cannot exceed 5MB");
                        ViewBag.LecturerName = user.FullName;
                        ViewBag.HourlyRate = user.HourlyRate;
                        return View(claim);
                    }

                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".png", ".jpg", ".jpeg" };
                    var extension = Path.GetExtension(document.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Only PDF, DOCX, XLSX, PNG, JPG files are allowed");
                        claim.IsDocumentValid = false;
                        ViewBag.LecturerName = user.FullName;
                        ViewBag.HourlyRate = user.HourlyRate;
                        return View(claim);
                    }

                    claim.IsDocumentValid = true;

                    // Save file
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + extension;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    claim.DocumentPath = "/uploads/" + uniqueFileName;
                }
                else
                {
                    claim.IsDocumentValid = false;
                }

                claim.Status = "Pending";
                claim.SubmittedDate = DateTime.Now;

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Claim submitted successfully! Total Amount: R{totalAmount:F2}";
                return RedirectToAction("ClaimStatus");
            }

            ViewBag.LecturerName = user.FullName;
            ViewBag.HourlyRate = user.HourlyRate;
            return View(claim);
        }

        public IActionResult ClaimStatus()
        {
            var userId = HttpContext.Session.GetUserId();
            var claims = _context.Claims
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            return View(claims);
        }
    }
}