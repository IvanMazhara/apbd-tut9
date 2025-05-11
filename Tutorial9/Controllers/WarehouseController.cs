using Microsoft.AspNetCore.Mvc;
using Tutorial9.Services;
using Tutorial9.Model.DTOs;

namespace Tutorial9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWarehouse(int id)
    {
        var warehouse = await _warehouseService.GetWarehouse(id);
        if (warehouse is null)
            return NotFound();

        return Ok(warehouse);
    }

    [HttpGet("{id}/exists")]
    public async Task<IActionResult> CheckWarehouse(int id)
    {
        var exists = await _warehouseService.WithGivenIdExists(id);
        return Ok(exists);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse(NewProductToWarehouseDto request)
    {
        if (request.Amount <= 0)
            return BadRequest("Amount must be greater than 0.");

        var success = await _warehouseService.AddProduct(request);
    
        if (success is null)
            return BadRequest("Validation failed or data not found.");

        return Ok(success);
    }
}