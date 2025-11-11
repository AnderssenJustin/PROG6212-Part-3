using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PROG6212_PART_3.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Claim> Claims { get; set; }
    }
}
