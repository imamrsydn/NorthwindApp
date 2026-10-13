using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Territory;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "SuperAdmin")]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionProvider _regionProvider;

        public RegionsController(IRegionProvider regionProvider)
        {
            _regionProvider = regionProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _regionProvider.GetAllRegionsAsync();
            return Ok(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] RegionFilterDto filter)
        {
            var result = await _regionProvider.GetPagedRegionsAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _regionProvider.GetRegionByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRegionDto request)
        {
            var result = await _regionProvider.CreateRegionAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _regionProvider.DeleteRegionAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}