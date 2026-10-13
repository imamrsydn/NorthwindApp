using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Territory;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "SuperAdmin")]
    public class TerritoriesController : ControllerBase
    {
        private readonly ITerritoryProvider _territoryProvider;

        public TerritoriesController(ITerritoryProvider territoryProvider)
        {
            _territoryProvider = territoryProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? regionId)
        {
            var result = await _territoryProvider.GetTerritoriesAsync(regionId);
            return Ok(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] TerritoryFilterDto filter)
        {
            var result = await _territoryProvider.GetPagedTerritoriesAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _territoryProvider.GetTerritoryByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTerritoryDto request)
        {
            var result = await _territoryProvider.CreateTerritoryAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.TerritoryId }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _territoryProvider.DeleteTerritoryAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}