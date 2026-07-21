using HealthcareCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStats> GetStats()
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            var totalPatients = await _context.Patients.CountAsync();
            var totalDoctors = await _context.Doctors.CountAsync(d => d.IsActive);
            var totalAppointments = await _context.Appointments.CountAsync();

            var appointmentsToday = await _context.Appointments
                .CountAsync(a => a.AppointmentDate.Date == today);

            var appointmentsThisWeek = await _context.Appointments
                .CountAsync(a => a.AppointmentDate.Date >= weekStart);

            var pendingCount = await _context.Appointments
                .CountAsync(a => a.Status == "Pending");

            var confirmedCount = await _context.Appointments
                .CountAsync(a => a.Status == "Confirmed");

            var cancelledCount = await _context.Appointments
                .CountAsync(a => a.Status == "Cancelled");

            return new DashboardStats
            {
                TotalPatients = totalPatients,
                TotalDoctors = totalDoctors,
                TotalAppointments = totalAppointments,
                AppointmentsToday = appointmentsToday,
                AppointmentsThisWeek = appointmentsThisWeek,
                PendingCount = pendingCount,
                ConfirmedCount = confirmedCount,
                CancelledCount = cancelledCount
            };
        }
    }

    public class DashboardStats
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int AppointmentsToday { get; set; }
        public int AppointmentsThisWeek { get; set; }
        public int PendingCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int CancelledCount { get; set; }
    }
}