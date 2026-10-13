using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Customer;

namespace Noerthwind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "SuperAdmin")] // Fitur khusus SuperAdmin
public class CustomerTypesController : ControllerBase
{
    private readonly ICustomerTypeProvider _provider;

    public CustomerTypesController(ICustomerTypeProvider provider)
    {
        _provider = provider;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _provider.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerTypeDto req) => Ok(await _provider.CreateAsync(req));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) => Ok(await _provider.DeleteAsync(id));
}