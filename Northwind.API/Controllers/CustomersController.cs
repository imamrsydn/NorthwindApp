using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Customer;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "SuperAdmin,Employee")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerProvider _customerProvider;

        public CustomersController(ICustomerProvider customerProvider)
        {
            _customerProvider = customerProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? companyName,
            [FromQuery] string? contactPerson,
            [FromQuery] string? region)
        {
            var result = await _customerProvider.GetAllCustomersAsync(companyName, contactPerson, region);
            return Ok(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] CustomerFilterDto filter)
        {
            var result = await _customerProvider.GetPagedCustomersAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _customerProvider.GetCustomerByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto request)
        {
            var result = await _customerProvider.CreateCustomerAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.CustomerId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCustomerDto request)
        {
            if (!id.Equals(request.CustomerId, StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "ID di URL dan di body tidak cocok." });

            var result = await _customerProvider.UpdateCustomerAsync(request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // Khusus SuperAdmin (SA)
        //[Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _customerProvider.DeleteCustomerAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}