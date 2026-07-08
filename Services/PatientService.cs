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

        // Get all patients (paged)
        public async Task<PagedResult<Patient>> GetAllPatients(int pageNumber, int pageSize)
        {
            var query = _context.Patients.OrderByDescending(p => p.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Patient>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Search patients by name, phone or date of birth
        public async Task<PagedResult<Patient>> SearchPatients(string search, int pageNumber, int pageSize)
        {
            var lowerSearch = search.ToLower();
            var query = _context.Patients
                .Where(p =>
                    p.FullName.ToLower().Contains(lowerSearch) ||
                    p.Phone.Contains(search) ||
                    p.DateOfBirth.ToString().Contains(search))
                .OrderByDescending(p => p.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Patient>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
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

        // Delete patient 
        public async Task<bool> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}