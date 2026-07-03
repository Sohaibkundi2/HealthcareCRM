using HealthcareCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Services
{
    public class PatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        // Get all patients
        public async Task<List<Patient>> GetAllPatients()
        {
            return await _context.Patients
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // Search patients by name
        public async Task<List<Patient>> SearchPatients(string search)
        {
            return await _context.Patients
                .Where(p => p.FullName.Contains(search))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // Get patient by id
        public async Task<Patient?> GetPatientById(int id)
        {
            return await _context.Patients.FindAsync(id);
        }

        // Add new patient
        public async Task<Patient> AddPatient(Patient patient)
        {
            patient.CreatedAt = DateTime.UtcNow;
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        // Update patient
        public async Task<bool> UpdatePatient(int id, Patient updated)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return false;

            patient.FullName = updated.FullName;
            patient.Email = updated.Email;
            patient.Phone = updated.Phone;
            patient.DateOfBirth = updated.DateOfBirth;
            patient.Gender = updated.Gender;
            patient.Address = updated.Address;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}