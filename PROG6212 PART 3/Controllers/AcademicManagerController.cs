using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_PART_3.Models;
using PROG6212_PART_3.Helpers;

namespace PROG6212_PART_3.Controllers
{
    [SessionAuthorize(Roles = new[] { "AcademicManager" })]
    public class AcademicManagerController : Controller
    {
        private readonly AppDbContext _context;

        public AcademicManagerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ClaimApproval()
        {
            // Show only claims approved by coordinator and pending manager approval
            var managerPendingClaims = _context.Claims
                .Include(c => c.User)
                .Where(c => c.Status == "ManagerPending")
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            // Automated verification check
            foreach (var claim in managerPendingClaims)
            {
                claim.IsAmountValid = AutomatedAmountCheck(claim);
                claim.IsHoursValid = AutomatedHoursCheck(claim);
                claim.IsDocumentValid = !string.IsNullOrEmpty(claim.DocumentPath);
            }

            return View(managerPendingClaims);
        }

        // Automated verification methods
        private bool AutomatedAmountCheck(Claim claim)
        {
            var totalAmount = claim.HoursWorked * claim.HourlyRate;

            // Check if amount is within acceptable range
            if (totalAmount < 50 || totalAmount > 50000)
                return false;

            // Check if hourly rate is reasonable
            if (claim.HourlyRate < 50 || claim.HourlyRate > 1000)
                return false;

            return true;
        }

        private bool AutomatedHoursCheck(Claim claim)
        {
            // Check if hours are within acceptable range (monthly limit is 180)
            if (claim.HoursWorked < 0.1 || claim.HoursWorked > 180)
                return false;

            return true;
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            var claim = await _context.Claims.Include(c => c.User).FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim != null && claim.Status == "ManagerPending")
            {
                // Automated checks before approval
                var automatedChecks = PerformAutomatedChecks(claim);

                if (!automatedChecks.IsValid)
                {
                    TempData["Error"] = $"Automated check failed: {automatedChecks.ErrorMessage}";
                    return RedirectToAction("ClaimApproval");
                }

                claim.Status = "Approved"; // Final approval
                claim.ManagerApprovedDate = DateTime.Now;
                claim.ApprovedByManager = HttpContext.Session.GetFullName();

                await _context.SaveChangesAsync();

                TempData["Success"] = "Claim approved and finalized!";
            }

            return RedirectToAction("ClaimApproval");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int claimId, string rejectionReason)
        {
            var claim = await _context.Claims.FindAsync(claimId);

            if (claim != null && claim.Status == "ManagerPending")
            {
                claim.Status = "Rejected";
                claim.RejectionReason = rejectionReason ?? "Does not meet approval criteria";
                await _context.SaveChangesAsync();

                TempData["Success"] = "Claim rejected!";
            }

            return RedirectToAction("ClaimApproval");
        }

        // Automated verification system
        private (bool IsValid, string ErrorMessage) PerformAutomatedChecks(Claim claim)
        {
            // Check 1: Document required
            if (string.IsNullOrEmpty(claim.DocumentPath))
            {
                return (false, "Supporting document is required");
            }

            // Check 2: Amount validation
            if (!AutomatedAmountCheck(claim))
            {
                return (false, "Amount or rate outside acceptable range");
            }

            // Check 3: Hours validation (180 hour monthly limit)
            if (!AutomatedHoursCheck(claim))
            {
                return (false, "Hours worked outside acceptable range (max 180 hours per month)");
            }

            // Check 4: User validation
            if (claim.User == null)
            {
                return (false, "User information is required");
            }

            return (true, "All checks passed");
        }
    }
}