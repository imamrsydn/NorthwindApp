using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Product;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "SuperAdmin,Employee")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductProvider _productProvider;

        public ProductsController(IProductProvider productProvider)
        {
            _productProvider = productProvider;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll([FromBody] ProductFilterDto filter)
        {
            var result = await _productProvider.GetAllProductsAsync(filter.CategoryId, filter.SupplierId, filter.Discontinued, filter.ReadyToOrder);
            return Ok(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] ProductFilterDto filter)
        {
            var result = await _productProvider.GetPagedProductsAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productProvider.GetProductByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto request)
        {
            var result = await _productProvider.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.ProductId }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto request)
        {
            if (id != request.ProductId)
                return BadRequest(new { message = "ID di URL dan body tidak cocok." });

            var result = await _productProvider.UpdateProductAsync(request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productProvider.DeleteProductAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Toggle Active/Discontinue status (Khusus SuperAdmin)
        //[Authorize(Roles = "SuperAdmin")]
        [HttpPatch("{id:int}/discontinue")]
        public async Task<IActionResult> ToggleDiscontinue(int id, [FromQuery] bool discontinued)
        {
            var result = await _productProvider.ToggleDiscontinueAsync(id, discontinued);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Poin 2: Otomatisasi Reorder Stok Produk
        [HttpPost("reorder")]
        public async Task<IActionResult> Reorder([FromBody] ReorderProductDto request)
        {
            var result = await _productProvider.ReorderProductAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Poin 3: Update UnitOnOrder ke UnitInStock (Barang Datang)
        [HttpPost("receive-stock")]
        public async Task<IActionResult> ReceiveStock()
        {
            var result = await _productProvider.ReceiveStockFromOrderAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}