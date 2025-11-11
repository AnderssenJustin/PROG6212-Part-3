using System;
using Microsoft.AspNetCore.Mvc;
using PROG6212_PART_3.Models;

namespace PROG6212_PART_3.Controllers
{

    public class ProgrammeCoOrdinatorController : Controller
    {
        private readonly AppDbContext _context;

        public ProgrammeCoOrdinatorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ApproveClaim()
        {
            var coordinatorPendingClaims = _context.Claims
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            // Automated verification check
            foreach (var claim in coordinatorPendingClaims)
            {
                claim.IsAmountValid = AutomatedAmountCheck(claim);
                claim.IsHoursValid = AutomatedHoursCheck(claim);
                claim.IsDocumentValid = !string.IsNullOrEmpty(claim.DocumentPath);
            }

            return View(coordinatorPendingClaims);
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
            // Check if hours are within acceptable range
            if (claim.HoursWorked < 0.1 || claim.HoursWorked > 200)
                return false;

            // Check if hours per day are reasonable (assuming max 16 hours/day)
            if (claim.HoursWorked > 16)
            {
                // Flag for review but don't auto-reject
                return true;
            }

            return true;
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            var claim = await _context.Claims.FindAsync(claimId);

            if (claim != null && claim.Status == "Pending")
            {
                // Automated checks before approval
                var automatedChecks = PerformAutomatedChecks(claim);

                if (!automatedChecks.IsValid)
                {
                    TempData["Error"] = $"Automated check failed: {automatedChecks.ErrorMessage}";
                    return RedirectToAction("ApproveClaim");
                }

                claim.Status = "ManagerPending";
                await _context.SaveChangesAsync();

                TempData["Success"] = "Claim approved and sent to Academic Manager!";
            }

            return RedirectToAction("ApproveClaim");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int claimId, string rejectionReason)
        {
            var claim = await _context.Claims.FindAsync(claimId);

            if (claim != null && claim.Status == "Pending")
            {
                claim.Status = "Rejected";
                claim.RejectionReason = rejectionReason ?? "Does not meet approval criteria";
                await _context.SaveChangesAsync();

                TempData["Success"] = "Claim rejected!";
            }

            return RedirectToAction("ApproveClaim");
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

            // Check 3: Hours validation
            if (!AutomatedHoursCheck(claim))
            {
                return (false, "Hours worked outside acceptable range");
            }

            // Check 4: Lecturer name validation
            if (string.IsNullOrWhiteSpace(claim.LecturerName))
            {
                return (false, "Lecturer name is required");
            }

            return (true, "All checks passed");
        }
    }

}
