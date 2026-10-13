using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Category;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "SuperAdmin,Employee")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryProvider _categoryProvider;

        public CategoriesController(ICategoryProvider categoryProvider)
        {
            _categoryProvider = categoryProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? name)
        {
            var result = await _categoryProvider.GetAllCategoriesAsync(name);
            return Ok(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] CategoryFilterDto filter)
        {
            var result = await _categoryProvider.GetPagedCategoriesAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryProvider.GetCategoryByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto request)
        {
            var result = await _categoryProvider.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.CategoryId }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto request)
        {
            if (id != request.CategoryId)
            {
                return BadRequest(new { message = "ID di URL dan di body tidak cocok." });
            }

            var result = await _categoryProvider.UpdateCategoryAsync(request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryProvider.DeleteCategoryAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("Top3")]
        public async Task<IActionResult> getTop3Product([FromQuery]int id, [FromQuery] int limit)
        {
            var result = await _categoryProvider.GetTopProduct(limit, id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}