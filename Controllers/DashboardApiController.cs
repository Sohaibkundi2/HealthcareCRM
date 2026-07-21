using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Services;

namespace HealthcareCRM.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardApiController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardApiController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET /api/dashboard/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _dashboardService.GetStats();
            return Ok(new { success = true, data = stats });
        }
    }
}