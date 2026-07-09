using HealthcareCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Services
{
    public class DoctorService
    {
        private readonly AppDbContext _context;

        public DoctorService(AppDbContext context)
        {
            _context = context;
        }

        // Get all active doctors
        public async Task<List<Doctor>> GetAllDoctors()
        {
            return await _context.Doctors
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }
}