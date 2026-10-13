using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Supplier;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "SuperAdmin,Employee")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierProvider _supplierProvider;

        public SuppliersController(ISupplierProvider supplierProvider)
        {
            _supplierProvider = supplierProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] string? city, [FromQuery] string? country)
        {
            var result = await _supplierProvider.GetAllSuppliersAsync(name, city, country);
            return Ok(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] SupplierFilterDto filter)
        {
            var result = await _supplierProvider.GetPagedSuppliersAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _supplierProvider.GetSupplierByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto request)
        {
            var result = await _supplierProvider.CreateSupplierAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.SupplierId }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierDto request)
        {
            if (id != request.SupplierId)
            {
                return BadRequest(new { message = "ID di URL dan di body tidak cocok." });
            }

            var result = await _supplierProvider.UpdateSupplierAsync(request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // Khusus SuperAdmin yang bisa menghapus Supplier (SA-only)
        //[Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _supplierProvider.DeleteSupplierAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}