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

        // Get all doctors including inactive
        public async Task<List<Doctor>> GetAllDoctorsIncludingInactive()
        {
            return await _context.Doctors
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        // Get doctor by id
        public async Task<Doctor?> GetDoctorById(int id)
        {
            return await _context.Doctors.FindAsync(id);
        }

        // Add new doctor
        public async Task<Doctor> AddDoctor(Doctor doctor)
        {
            doctor.CreatedAt = DateTime.UtcNow;
            doctor.IsActive = true;
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        // Update doctor
        public async Task<bool> UpdateDoctor(int id, Doctor updated)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return false;

            doctor.Name = updated.Name;
            doctor.Specialization = updated.Specialization;
            doctor.Phone = updated.Phone;

            await _context.SaveChangesAsync();
            return true;
        }

        // Deactivate doctor (soft delete)
        public async Task<bool> DeactivateDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return false;

            doctor.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Reactivate doctor
        public async Task<bool> ReactivateDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return false;

            doctor.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}