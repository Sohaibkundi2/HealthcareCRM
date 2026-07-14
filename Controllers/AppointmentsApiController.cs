using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using HealthcareCRM.Services;

namespace HealthcareCRM.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentsApiController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // GET /api/appointments?status=Pending
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var appointments = await _appointmentService.GetAllAppointments(status);
            return Ok(new { success = true, data = appointments });
        }

        // GET /api/appointments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentById(id);
            if (appointment == null)
                return NotFound(new { success = false, message = "Appointment not found" });

            return Ok(new { success = true, data = appointment });
        }

        // POST /api/appointments
        [HttpPost]
        public async Task<IActionResult> Book([FromBody] Appointment appointment)
        {
            if (appointment.PatientId == 0)
                return BadRequest(new { success = false, message = "Patient is required" });

            if (appointment.DoctorId == 0)
                return BadRequest(new { success = false, message = "Doctor is required" });

            if (appointment.AppointmentDate == default)
                return BadRequest(new { success = false, message = "Appointment date is required" });

            if (appointment.AppointmentDate < DateTime.UtcNow)
                return BadRequest(new { success = false, message = "Appointment date cannot be in the past" });

            var created = await _appointmentService.BookAppointment(appointment);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { success = true, data = created });
        }

        // PUT /api/appointments/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var validStatuses = new[] { "Pending", "Confirmed", "Cancelled" };
            if (!validStatuses.Contains(request.Status))
                return BadRequest(new { success = false, message = "Invalid status. Must be Pending, Confirmed or Cancelled" });

            var success = await _appointmentService.UpdateStatus(id, request.Status);
            if (!success)
                return NotFound(new { success = false, message = "Appointment not found" });

            return Ok(new { success = true, message = "Status updated successfully" });
        }

        // PUT /api/appointments/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _appointmentService.CancelAppointment(id);
            if (!success)
                return NotFound(new { success = false, message = "Appointment not found" });

            return Ok(new { success = true, message = "Appointment cancelled successfully" });
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}