using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Users table
        public DbSet<User> Users { get; set; }
    }
}