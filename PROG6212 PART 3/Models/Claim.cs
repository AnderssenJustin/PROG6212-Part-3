using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_PART_3.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        // Foreign key to User
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Lecturer info is now pulled from User table
        [Display(Name = "Lecturer Name")]
        public string LecturerName => User != null ? User.FullName : "";

        [Required(ErrorMessage = "Please enter hours worked")]
        [Range(0.1, 180, ErrorMessage = "Hours must be between 0.1 and 180 (monthly limit)")]
        [Display(Name = "Hours Worked")]
        public double HoursWorked { get; set; }

        // Hourly rate is now pulled from User table
        [Display(Name = "Hourly Rate")]
        public double HourlyRate => User != null ? User.HourlyRate : 0;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        [Display(Name = "Supporting Document")]
        public string? DocumentPath { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, CoordinatorApproved, ManagerApproved, Rejected

        [Display(Name = "Submitted Date")]
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        [Display(Name = "Coordinator Approved Date")]
        public DateTime? CoordinatorApprovedDate { get; set; }

        [Display(Name = "Manager Approved Date")]
        public DateTime? ManagerApprovedDate { get; set; }

        [Display(Name = "Rejection Reason")]
        [StringLength(500)]
        public string? RejectionReason { get; set; }

        [Display(Name = "Total Amount")]
        public double TotalAmount
        {
            get { return HoursWorked * HourlyRate; }
        }

        // Validation flags
        public bool IsDocumentValid { get; set; } = false;
        public bool IsAmountValid { get; set; } = false;
        public bool IsHoursValid { get; set; } = false;

        // Approval tracking
        [MaxLength(100)]
        public string? ApprovedByCoordinator { get; set; }

        [MaxLength(100)]
        public string? ApprovedByManager { get; set; }
    }
}
