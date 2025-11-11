using System;
using Microsoft.AspNetCore.Mvc;
using PROG6212_PART_3.Models;

namespace PROG6212_PART_3.Controllers
{
    public class AcademicManagerController : Controller
    {
        private readonly AppDbContext _context;

        public AcademicManagerController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult ClaimApproval()
        {
            // Show only claims approved by coordinator and pending manager approval
            var managerPendingClaims = _context.Claims
                .Where(c => c.Status == "ManagerPending")
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            return View(managerPendingClaims);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim != null && claim.Status == "ManagerPending")
            {
                claim.Status = "Approved"; // Final approval
                await _context.SaveChangesAsync();
                TempData["Success"] = "Claim approved";
            }
            return RedirectToAction("ClaimApproval");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int claimId)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim != null && claim.Status == "ManagerPending")
            {
                claim.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Claim rejected!";
            }
            return RedirectToAction("ClaimApproval");
        }
    }
}
