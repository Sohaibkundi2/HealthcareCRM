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
        private const int DefaultPageSize = 10;

        public PatientsApiController(PatientService patientService)
        {
            _patientService = patientService;
        }

        // GET /api/patients?search=&pageNumber=&pageSize=
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;

            var result = string.IsNullOrWhiteSpace(search)
                ? await _patientService.GetAllPatients(pageNumber, pageSize)
                : await _patientService.SearchPatients(search, pageNumber, pageSize);

            return Ok(new
            {
                success = true,
                data = result.Items,
                totalCount = result.TotalCount,
                pageNumber = result.PageNumber,
                pageSize = result.PageSize,
                totalPages = result.TotalPages
            });
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

        // DELETE /api/patients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _patientService.DeletePatient(id);

            if (!success)
                return NotFound(new { success = false, message = "Patient not found" });

            return Ok(new { success = true, message = "Patient deleted successfully" });
        }
    }
}