using System.Text;
using Microsoft.AspNetCore.Mvc;
using PROG6212_PART_3.Models;

namespace PROG6212_PART_3.Controllers
{
    public class HRController : Controller
    {
        private readonly AppDbContext _context;

        public HRController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult HRDashboard()
        {
            var approvedClaims = _context.Claims
                .Where(c => c.Status == "Approved")
                .OrderByDescending(c => c.ApprovedDate ?? c.SubmittedDate)
                .ToList();

            return View(approvedClaims);
        }

        // Automated invoice generation
        public IActionResult GenerateInvoice(int claimId)
        {
            var claim = _context.Claims.Find(claimId);

            if (claim == null || claim.Status != "Approved")
            {
                TempData["Error"] = "Claim not found or not approved";
                return RedirectToAction("Dashboard");
            }

            var invoice = GenerateInvoiceReport(claim);

            var bytes = Encoding.UTF8.GetBytes(invoice);
            return File(bytes, "text/plain", $"Invoice_{claim.ClaimId}_{DateTime.Now:yyyyMMdd}.txt");
        }

        // Automated report generation
        public IActionResult GenerateMonthlyReport()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var claims = _context.Claims
                .Where(c => c.Status == "Approved"
                    && c.SubmittedDate.Month == currentMonth
                    && c.SubmittedDate.Year == currentYear)
                .ToList();

            var report = GeneratePaymentReport(claims, currentMonth, currentYear);

            var bytes = Encoding.UTF8.GetBytes(report);
            return File(bytes, "text/plain", $"Monthly_Report_{currentYear}_{currentMonth:D2}.txt");
        }

        // Generate summary report for all approved claims
        public IActionResult GenerateSummaryReport()
        {
            var approvedClaims = _context.Claims
                .Where(c => c.Status == "Approved")
                .ToList();

            var report = GenerateSummaryReportContent(approvedClaims);

            var bytes = Encoding.UTF8.GetBytes(report);
            return File(bytes, "text/plain", $"Summary_Report_{DateTime.Now:yyyyMMdd}.txt");
        }

        // Helper method to generate invoice
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
            sb.AppendLine($"Name: {claim.LecturerName}");
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

        // Helper method to generate monthly report
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

            var lecturerGroups = claims.GroupBy(c => c.LecturerName);

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

        // Helper method to generate summary report
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
                sb.AppendLine($"  Lecturer: {claim.LecturerName}");
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