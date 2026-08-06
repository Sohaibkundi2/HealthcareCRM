using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using HealthcareCRM.Services;
using HealthcareCRM.Helpers;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace HealthcareCRM.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        private readonly ILogger<AppointmentsApiController> _logger;

        public AppointmentsApiController(
            AppointmentService appointmentService,
            ILogger<AppointmentsApiController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        // GET /api/appointments?status=Pending
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointments(status);
                return Ok(new { success = true, data = appointments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments (status={Status})", status);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving appointments" });
            }
        }

        // GET /api/appointments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentById(id);
                if (appointment == null)
                    return NotFound(new { success = false, message = "Appointment not found" });

                return Ok(new { success = true, data = appointment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the appointment" });
            }
        }

        // POST /api/appointments
        [HttpPost]
        public async Task<IActionResult> Book([FromBody] Appointment appointment)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking appointment");
                return StatusCode(500, new { success = false, message = "An error occurred while booking the appointment" });
            }
        }

        // PUT /api/appointments/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var validStatuses = new[] { "Pending", "Confirmed", "Cancelled" };
                if (!validStatuses.Contains(request.Status))
                    return BadRequest(new { success = false, message = "Invalid status. Must be Pending, Confirmed or Cancelled" });

                var success = await _appointmentService.UpdateStatus(id, request.Status);
                if (!success)
                    return NotFound(new { success = false, message = "Appointment not found" });

                return Ok(new { success = true, message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for appointment {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while updating the status" });
            }
        }

        // PUT /api/appointments/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var success = await _appointmentService.CancelAppointment(id);
                if (!success)
                    return NotFound(new { success = false, message = "Appointment not found" });

                return Ok(new { success = true, message = "Appointment cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while cancelling the appointment" });
            }
        }

        // GET /api/appointments/report?from=&to=
        [HttpGet("report")]
        [RequireRole("Admin")]
        public async Task<IActionResult> Report([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
                var toDate = to ?? DateTime.UtcNow;

                if (fromDate > toDate)
                    return BadRequest(new { success = false, message = "'from' date cannot be later than 'to' date" });

                var appointments = await _appointmentService.GetAppointmentsByDateRange(fromDate, toDate);

                byte[] pdfBytes;

                using (var stream = new MemoryStream())
                {
                    using (var writer = new PdfWriter(stream))
                    using (var pdf = new PdfDocument(writer))
                    using (var document = new Document(pdf))
                    {
                        var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                        var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                        // Title
                        document.Add(new Paragraph("HealthcareCRM — Appointment Report")
                            .SetFont(titleFont)
                            .SetFontSize(18)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMarginBottom(5));

                        document.Add(new Paragraph($"Period: {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}")
                            .SetFont(normalFont)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontColor(ColorConstants.GRAY)
                            .SetMarginBottom(20));

                        // Summary
                        document.Add(new Paragraph("Summary")
                            .SetFont(titleFont)
                            .SetFontSize(13)
                            .SetMarginBottom(5));

                        var pending = appointments.Count(a => a.Status == "Pending");
                        var confirmed = appointments.Count(a => a.Status == "Confirmed");
                        var cancelled = appointments.Count(a => a.Status == "Cancelled");

                        document.Add(new Paragraph($"Total Appointments: {appointments.Count}")
                            .SetFont(normalFont).SetFontSize(10));
                        document.Add(new Paragraph($"Confirmed: {confirmed}  |  Pending: {pending}  |  Cancelled: {cancelled}")
                            .SetFont(normalFont).SetFontSize(10).SetMarginBottom(20));

                        // Table
                        document.Add(new Paragraph("Appointments")
                            .SetFont(titleFont)
                            .SetFontSize(13)
                            .SetMarginBottom(5));

                        var table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 3, 3, 2 }))
                            .UseAllAvailableWidth();

                        var headerColor = new DeviceRgb(0, 255, 136);
                        foreach (var header in new[] { "Patient", "Doctor", "Date", "Status" })
                        {
                            table.AddHeaderCell(new Cell()
                                .Add(new Paragraph(header).SetFont(titleFont).SetFontSize(9))
                                .SetBackgroundColor(new DeviceRgb(17, 17, 17))
                                .SetFontColor(headerColor));
                        }

                        foreach (var a in appointments)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(a.Patient?.FullName ?? "—").SetFont(normalFont).SetFontSize(9)));
                            table.AddCell(new Cell().Add(new Paragraph(a.Doctor?.Name ?? "—").SetFont(normalFont).SetFontSize(9)));
                            table.AddCell(new Cell().Add(new Paragraph(a.AppointmentDate.ToString("yyyy-MM-dd HH:mm")).SetFont(normalFont).SetFontSize(9)));
                            table.AddCell(new Cell().Add(new Paragraph(a.Status).SetFont(normalFont).SetFontSize(9)));
                        }

                        document.Add(table);

                        document.Add(new Paragraph($"\nTotal: {appointments.Count} appointments")
                            .SetFont(titleFont)
                            .SetFontSize(10)
                            .SetMarginTop(10));

                        document.Close();
                    }

                    pdfBytes = stream.ToArray();
                }

                return File(pdfBytes, "application/pdf",
                    $"appointments_report_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating appointment report (from={From}, to={To})", from, to);
                return StatusCode(500, new { success = false, message = "An error occurred while generating the report" });
            }
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}