using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // We'll add DbSet properties here as we create models (e.g. Patients, Users)
    }
}