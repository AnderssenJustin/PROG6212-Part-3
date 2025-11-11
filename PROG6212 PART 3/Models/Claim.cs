using System.ComponentModel.DataAnnotations;

namespace PROG6212_PART_3.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [Required]
        public string LecturerName { get; set; }

        [Required]
        public double HoursWorked { get; set; }

        [Required]
        public double HourlyRate { get; set; }

        public string? Notes { get; set; }

        public string? DocumentName { get; set; }

        public string? DocumentPath { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, ManagerApproved, Approved, Rejected

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        public double CalculateTotalAmount()
        {
            return HoursWorked * HourlyRate;
        }

    }
}
