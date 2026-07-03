using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using HealthcareCRM.Services;

namespace HealthcareCRM.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsApiController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientsApiController(PatientService patientService)
        {
            _patientService = patientService;
        }

        // GET /api/patients
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var patients = string.IsNullOrWhiteSpace(search)
                ? await _patientService.GetAllPatients()
                : await _patientService.SearchPatients(search);

            return Ok(new { success = true, data = patients });
        }

        // GET /api/patients/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetPatientById(id);

            if (patient == null)
                return NotFound(new { success = false, message = "Patient not found" });

            return Ok(new { success = true, data = patient });
        }

        // POST /api/patients
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input" });

            var created = await _patientService.AddPatient(patient);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { success = true, data = created });
        }

        // PUT /api/patients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input" });

            var success = await _patientService.UpdatePatient(id, patient);

            if (!success)
                return NotFound(new { success = false, message = "Patient not found" });

            return Ok(new { success = true, message = "Patient updated successfully" });
        }
    }
}