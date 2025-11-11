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
                // Show only claims pending coordinator approval
                var coordinatorPendingClaims = _context.Claims
                    .Where(c => c.Status == "Pending")
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToList();

                return View(coordinatorPendingClaims);
            }

            [HttpPost]
            // logic for programme co ordinator to approve claim 
            public async Task<IActionResult> ApproveClaim(int claimId)
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim != null && claim.Status == "Pending")
                {
                    claim.Status = "ManagerPending";
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Claim approved and sent to Academic Manager!";
                }
                return RedirectToAction("ApproveClaim");
            }

            [HttpPost]
            // logic to reject a claim 
            public async Task<IActionResult> RejectClaim(int claimId)
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim != null && claim.Status == "Pending")
                {
                    claim.Status = "Rejected";
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Claim rejected!";
                }
                return RedirectToAction("ApproveClaim");
            }
        
}
