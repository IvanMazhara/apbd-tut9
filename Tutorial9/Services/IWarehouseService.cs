using Tutorial9.Model.DTOs;

namespace Tutorial9.Services;

public interface IWarehouseService
{
    Task<bool> WithGivenIdExists(int Id);
    Task<WarehouseDto> GetWarehouse(int Id);
}