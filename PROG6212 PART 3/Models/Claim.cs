using System.ComponentModel.DataAnnotations;

namespace PROG6212_PART_3.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string LecturerName { get; set; }

        [Required(ErrorMessage = "Please enter hours worked")]
        [Range(0.1, 200, ErrorMessage = "Hours must be between 0.1 and 200")]
        public double HoursWorked { get; set; }

        [Required(ErrorMessage = "Please enter hourly rate")]
        [Range(50, 1000, ErrorMessage = "Hourly rate must be between R50 and R1000")]
        public double HourlyRate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public string? DocumentPath { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        public DateTime? ApprovedDate { get; set; }

        public string? RejectionReason { get; set; }


        public double TotalAmount
        {
            get { return HoursWorked * HourlyRate; }
        }


        public bool IsDocumentValid { get; set; } = false;
        public bool IsAmountValid { get; set; } = false;
        public bool IsHoursValid { get; set; } = false;
    }

}
