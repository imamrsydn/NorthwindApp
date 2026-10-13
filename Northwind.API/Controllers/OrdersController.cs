using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Order;

namespace Noerthwind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "SuperAdmin,Employee,Customer")]
public class OrdersController : ControllerBase
{
    private readonly IOrderProvider _orderProvider;

    public OrdersController(IOrderProvider orderProvider)
    {
        _orderProvider = orderProvider;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? customerId,
        [FromQuery] int? supplierId,
        [FromQuery] int? employeeId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var currentCustomerId = User.FindFirstValue("CustomerId");
        var currentEmployeeIdStr = User.FindFirstValue("EmployeeId");
        int? currentEmployeeId = int.TryParse(currentEmployeeIdStr, out var empId) ? empId : null;

        var result = await _orderProvider.GetAllOrdersAsync(
            customerId, supplierId, employeeId, startDate, endDate,
            userRole, currentCustomerId, currentEmployeeId);

        return Ok(result);
    }

    [HttpPost("paged")]
    public async Task<IActionResult> GetPaged([FromBody] OrderFilterDto filter)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var currentCustomerId = User.FindFirstValue("CustomerId");
        var currentEmployeeIdStr = User.FindFirstValue("EmployeeId");
        int? currentEmployeeId = int.TryParse(currentEmployeeIdStr, out var empId) ? empId : null;

        var result = await _orderProvider.GetPagedOrdersAsync(filter, userRole, currentCustomerId, currentEmployeeId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var currentCustomerId = User.FindFirstValue("CustomerId");
        var currentEmployeeIdStr = User.FindFirstValue("EmployeeId");
        int? currentEmployeeId = int.TryParse(currentEmployeeIdStr, out var empId) ? empId : null;

        var result = await _orderProvider.GetOrderByIdAsync(
            id, userRole, currentCustomerId, currentEmployeeId);

        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto request)
    {
        var result = await _orderProvider.CreateOrderAsync(request);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.OrderId }, result);
    }

    [HttpPatch("status")]
    //[Authorize(Roles = "SuperAdmin,Employee")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusDto request)
    {
        var result = await _orderProvider.UpdateOrderStatusAsync(request);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    //[Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _orderProvider.DeleteOrderAsync(id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}