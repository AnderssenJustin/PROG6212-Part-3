using Microsoft.AspNetCore.Mvc;
using PROG6212_PART_3.Models;
using PROG6212_PART_3;

namespace Prog6212_POE_ST10340607.Controllers
{
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
            var claims = _context.Claims.OrderByDescending(c => c.SubmittedDate).ToList();
            return View(claims);
        }

        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(PROG6212_PART_3.Models.Claim claim, IFormFile? document)
        {
            // Automated validation checks
            var validationErrors = new List<string>();

            // Check hours worked
            if (claim.HoursWorked < 0.1 || claim.HoursWorked > 200)
            {
                validationErrors.Add("Hours worked must be between 0.1 and 200");
                claim.IsHoursValid = false;
            }
            else
            {
                claim.IsHoursValid = true;
            }

            // Check hourly rate
            if (claim.HourlyRate < 50 || claim.HourlyRate > 1000)
            {
                validationErrors.Add("Hourly rate must be between R50 and R1000");
                claim.IsAmountValid = false;
            }
            else
            {
                claim.IsAmountValid = true;
            }

            // Auto-calculate total amount
            var totalAmount = claim.HoursWorked * claim.HourlyRate;

            // Check if total amount is reasonable (automated business rule)
            if (totalAmount > 50000)
            {
                validationErrors.Add("Total claim amount exceeds maximum limit of R50,000");
            }

            if (validationErrors.Any())
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(claim);
            }

            if (ModelState.IsValid)
            {
                // Automated file validation
                if (document != null && document.Length > 0)
                {
                    if (document.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "File size cannot exceed 5MB");
                        return View(claim);
                    }

                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".png", ".jpg", ".jpeg" };
                    var extension = Path.GetExtension(document.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Only PDF, DOCX, XLSX, PNG, JPG files are allowed");
                        claim.IsDocumentValid = false;
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

                claim.Status = "Pending";
                claim.SubmittedDate = DateTime.Now;

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Claim submitted successfully! Total Amount: R{totalAmount:F2}";
                return RedirectToAction("ClaimStatus");
            }

            return View(claim);
        }

        public IActionResult ClaimStatus()
        {
            var claims = _context.Claims.OrderByDescending(c => c.SubmittedDate).ToList();
            return View(claims);
        }
    }

}