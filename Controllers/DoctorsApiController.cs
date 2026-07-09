using Microsoft.AspNetCore.Mvc;
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

        // GET /api/doctors
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorService.GetAllDoctors();
            return Ok(new { success = true, data = doctors });
        }
    }
}