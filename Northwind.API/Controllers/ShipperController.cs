using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Shipper;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "SuperAdmin")] // Fitur khusus SuperAdmin
    public class ShippersController : ControllerBase
    {
        private readonly IShipperProvider _shipperProvider;

        public ShippersController(IShipperProvider shipperProvider)
        {
            _shipperProvider = shipperProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _shipperProvider.GetAllShippersAsync());

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] ShipperFilterDto filter)
        {
            var result = await _shipperProvider.GetPagedShippersAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _shipperProvider.GetShipperByIdAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShipperDto req)
        {
            var res = await _shipperProvider.CreateShipperAsync(req);
            return CreatedAtAction(nameof(GetById), new { id = res.Data?.ShipperId }, res);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShipperDto req)
        {
            if (id != req.ShipperId) return BadRequest(new { message = "ID mismatch." });
            var res = await _shipperProvider.UpdateShipperAsync(req);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _shipperProvider.DeleteShipperAsync(id);
            return res.Success ? Ok(res) : BadRequest(res);
        }
    }
}