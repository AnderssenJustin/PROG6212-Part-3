using Microsoft.EntityFrameworkCore;

namespace PROG6212_PART_3.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-Claim relationship
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.User)
                .WithMany(u => u.Claims)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed default HR user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "hradmin",
                    PasswordHash = "HR@2025",
                    Role = "HR",
                    FirstName = "HR",
                    LastName = "Administrator",
                    Email = "hr@1.com",
                    HourlyRate = 0,
                    IsActive = true,
                    CreatedDate = new DateTime(2025,11,16)
                }
            );
        }
    }
}
