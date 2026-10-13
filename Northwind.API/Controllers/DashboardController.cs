using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Employee")] // Akses SuperAdmin & Employee
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardProvider _dashboardProvider;

        public DashboardController(IDashboardProvider dashboardProvider)
        {
            _dashboardProvider = dashboardProvider;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _dashboardProvider.GetDashboardSummaryAsync();
            return Ok(result);
        }
    }
}