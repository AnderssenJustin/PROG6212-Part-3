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
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile? document)
        {
            if (ModelState.IsValid)
            {
                // Handles the file upload
                if (document != null && document.Length > 0)
                {
                    // Checks file size (max 5MB)
                    if (document.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "File size cannot exceed 5MB");
                        return View(claim);
                    }

                    // Checks the  file extension
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".png" };
                    var extension = Path.GetExtension(document.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Only PDF, DOCX,PNG and XLSX files are allowed");
                        return View(claim);
                    }

                    // Saves the  file
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
                //claim.Status = "CoordinatorPending";
                claim.Status = "Pending";
                claim.SubmittedDate = DateTime.Now;

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Claim submitted successfully!";
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