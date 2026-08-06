using HealthcareCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Services
{
    public class AppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsByDateRange(DateTime from, DateTime to)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.AppointmentDate.Date >= from.Date &&
                            a.AppointmentDate.Date <= to.Date)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        // Get all appointments with patient and doctor info
        public async Task<List<Appointment>> GetAllAppointments(string? status = null)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            return await query
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        // Get appointment by id
        public async Task<Appointment?> GetAppointmentById(int id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // Book new appointment
        public async Task<Appointment> BookAppointment(Appointment appointment)
        {
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.Status = "Pending";
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        // Update appointment status
        public async Task<bool> UpdateStatus(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            appointment.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        // Cancel appointment
        public async Task<bool> CancelAppointment(int id)
        {
            return await UpdateStatus(id, "Cancelled");
        }
    }
}