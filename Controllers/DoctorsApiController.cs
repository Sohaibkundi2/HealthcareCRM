using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using HealthcareCRM.Services;

namespace HealthcareCRM.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorsApiController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorsApiController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET /api/doctors?includeInactive=true
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var doctors = includeInactive
                ? await _doctorService.GetAllDoctorsIncludingInactive()
                : await _doctorService.GetAllDoctors();

            return Ok(new { success = true, data = doctors });
        }

        // GET /api/doctors/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorService.GetDoctorById(id);
            if (doctor == null)
                return NotFound(new { success = false, message = "Doctor not found" });

            return Ok(new { success = true, data = doctor });
        }

        // POST /api/doctors
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Doctor doctor)
        {
            if (string.IsNullOrWhiteSpace(doctor.Name))
                return BadRequest(new { success = false, message = "Name is required" });

            if (string.IsNullOrWhiteSpace(doctor.Specialization))
                return BadRequest(new { success = false, message = "Specialization is required" });

            if (string.IsNullOrWhiteSpace(doctor.Phone))
                return BadRequest(new { success = false, message = "Phone is required" });

            var created = await _doctorService.AddDoctor(doctor);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { success = true, data = created });
        }

        // PUT /api/doctors/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Doctor doctor)
        {
            if (string.IsNullOrWhiteSpace(doctor.Name))
                return BadRequest(new { success = false, message = "Name is required" });

            if (string.IsNullOrWhiteSpace(doctor.Specialization))
                return BadRequest(new { success = false, message = "Specialization is required" });

            if (string.IsNullOrWhiteSpace(doctor.Phone))
                return BadRequest(new { success = false, message = "Phone is required" });

            var success = await _doctorService.UpdateDoctor(id, doctor);
            if (!success)
                return NotFound(new { success = false, message = "Doctor not found" });

            return Ok(new { success = true, message = "Doctor updated successfully" });
        }

        // PUT /api/doctors/{id}/deactivate
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var success = await _doctorService.DeactivateDoctor(id);
            if (!success)
                return NotFound(new { success = false, message = "Doctor not found" });

            return Ok(new { success = true, message = "Doctor deactivated successfully" });
        }

        // PUT /api/doctors/{id}/reactivate
        [HttpPut("{id}/reactivate")]
        public async Task<IActionResult> Reactivate(int id)
        {
            var success = await _doctorService.ReactivateDoctor(id);
            if (!success)
                return NotFound(new { success = false, message = "Doctor not found" });

            return Ok(new { success = true, message = "Doctor reactivated successfully" });
        }
    }
}