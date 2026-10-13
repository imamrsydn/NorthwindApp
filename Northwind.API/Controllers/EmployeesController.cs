using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Employee;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "SuperAdmin")] // Fitur khusus SuperAdmin
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeProvider _employeeProvider;

        public EmployeesController(IEmployeeProvider employeeProvider)
        {
            _employeeProvider = employeeProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? employeeId,
            [FromQuery] string? fullName,
            [FromQuery] string? territory)
        {
            var result = await _employeeProvider.GetAllEmployeesAsync(employeeId, fullName, territory);
            return Ok(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] EmployeeFilterDto filter)
        {
            var result = await _employeeProvider.GetPagedEmployeesAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _employeeProvider.GetEmployeeByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto request)
        {
            var result = await _employeeProvider.CreateEmployeeAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.EmployeeId }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto request)
        {
            if (id != request.EmployeeId)
                return BadRequest(new { message = "ID di URL dan body tidak cocok." });

            var result = await _employeeProvider.UpdateEmployeeAsync(request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeeProvider.DeleteEmployeeAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Endpoint Mapping Territory ke Employee
        [HttpPost("assign-territories")]
        public async Task<IActionResult> AssignTerritories([FromBody] AssignTerritoriesDto request)
        {
            var result = await _employeeProvider.AssignTerritoriesAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}