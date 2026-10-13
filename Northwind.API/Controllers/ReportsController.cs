using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")] // Khusus SuperAdmin
    public class ReportsController : ControllerBase
    {
        private readonly IReportProvider _reportProvider;

        public ReportsController(IReportProvider reportProvider)
        {
            _reportProvider = reportProvider;
        }

        // groupBy options: byMonth, byProduct, byCategory, byCustomer, byEmployee
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] string groupBy = "byMonth", [FromQuery] int? year = null)
        {
            var result = await _reportProvider.GetSalesByGroupAsync(groupBy, year);
            return Ok(result);
        }
    }
}