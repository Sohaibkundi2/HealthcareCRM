using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using HealthcareCRM.Services;
using HealthcareCRM.Helpers;

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

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


        // GET /api/patients/export?format=csv
        [HttpGet("export")]
        [RequireRole("Admin")]
        public async Task<IActionResult> Export([FromQuery] string format = "csv")
        {
            var patients = await _patientService.GetAllPatientsForExport();

            if (format.ToLower() == "csv")
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

                // Write header
                csv.WriteHeader<PatientExportDto>();
                await csv.NextRecordAsync();

                // Write records
                foreach (var patient in patients)
                {
                    csv.WriteRecord(new PatientExportDto
                    {
                        Id = patient.Id,
                        FullName = patient.FullName,
                        Email = patient.Email,
                        Phone = patient.Phone,
                        Gender = patient.Gender,
                        DateOfBirth = patient.DateOfBirth.ToString("yyyy-MM-dd"),
                        Address = patient.Address,
                        RegisteredOn = patient.CreatedAt.ToString("yyyy-MM-dd")
                    });
                    await csv.NextRecordAsync();
                }

                await writer.FlushAsync();
                stream.Position = 0;

                return File(stream, "text/csv", $"patients_{DateTime.UtcNow:yyyyMMdd}.csv");
            }

            return BadRequest(new { success = false, message = "Unsupported format. Use format=csv" });
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
            if (string.IsNullOrWhiteSpace(patient.FullName))
                return BadRequest(new { success = false, message = "Full name is required" });

            if (string.IsNullOrWhiteSpace(patient.Phone))
                return BadRequest(new { success = false, message = "Phone is required" });

            if (patient.DateOfBirth == default)
                return BadRequest(new { success = false, message = "Date of birth is required" });

            if (string.IsNullOrWhiteSpace(patient.Gender))
                return BadRequest(new { success = false, message = "Gender is required" });

            if (patient.DateOfBirth > DateTime.UtcNow)
                return BadRequest(new { success = false, message = "Date of birth cannot be in the future" });

            var created = await _patientService.AddPatient(patient);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { success = true, data = created });
        }

        // PUT /api/patients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
        {
            if (string.IsNullOrWhiteSpace(patient.FullName))
                return BadRequest(new { success = false, message = "Full name is required" });

            if (string.IsNullOrWhiteSpace(patient.Phone))
                return BadRequest(new { success = false, message = "Phone is required" });

            if (patient.DateOfBirth == default)
                return BadRequest(new { success = false, message = "Date of birth is required" });

            if (string.IsNullOrWhiteSpace(patient.Gender))
                return BadRequest(new { success = false, message = "Gender is required" });

            if (patient.DateOfBirth > DateTime.UtcNow)
                return BadRequest(new { success = false, message = "Date of birth cannot be in the future" });

            var success = await _patientService.UpdatePatient(id, patient);

            if (!success)
                return NotFound(new { success = false, message = "Patient not found" });

            return Ok(new { success = true, message = "Patient updated successfully" });
        }

        // DELETE /api/patients/{id}
        [HttpDelete("{id}")]
        [RequireRole("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _patientService.DeletePatient(id);

            if (!success)
                return NotFound(new { success = false, message = "Patient not found" });

            return Ok(new { success = true, message = "Patient deleted successfully" });
        }
    }
}