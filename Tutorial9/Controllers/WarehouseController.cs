using Microsoft.AspNetCore.Mvc;
using Tutorial9.Services;

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
}