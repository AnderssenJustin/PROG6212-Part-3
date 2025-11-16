using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_PART_3.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(100)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(256)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Display(Name = "Role")]
        public string Role { get; set; } = "Lecturer"; // HR, Lecturer, ProgrammeCoordinator, AcademicManager

        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(50, 1000, ErrorMessage = "Hourly rate must be between R50 and R1000")]
        [Display(Name = "Hourly Rate")]
        public double HourlyRate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }

        // Navigation property for claims
        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
