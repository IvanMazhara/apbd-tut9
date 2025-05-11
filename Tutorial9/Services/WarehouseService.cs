using Microsoft.Data.SqlClient;
using Tutorial9.Model.DTOs;

namespace Tutorial9.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IConfiguration _configuration;

    public WarehouseService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> WithGivenIdExists(int Id)
    {
        var query = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @ID";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", Id);

        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<WarehouseDto> GetWarehouse(int Id)
    {
        var query = "SELECT IdWarehouse, Name, Address FROM Warehouse WHERE IdWarehouse = @ID";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", Id);
        
        await connection.OpenAsync();
        
        var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new WarehouseDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("IdWarehouse")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Address = reader.GetString(reader.GetOrdinal("Address")),
            };
        }

        return null;
    }
    
    public async Task<int?> AddProduct(NewProductToWarehouseDto request)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var checkProductCmd = new SqlCommand("SELECT 1 FROM Product WHERE IdProduct = @IdProduct", connection, (SqlTransaction)transaction);
            checkProductCmd.Parameters.AddWithValue("@IdProduct", request.IdProduct);
            if (await checkProductCmd.ExecuteScalarAsync() is null)
                return null;
            
            var checkWarehouseCmd = new SqlCommand("SELECT 1 FROM Warehouse WHERE IdWarehouse = @IdWarehouse", connection, (SqlTransaction)transaction);
            checkWarehouseCmd.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
            if (await checkWarehouseCmd.ExecuteScalarAsync() is null)
                return null;
            
            if (request.Amount <= 0)
                return null;
            
            var orderCheckCmd = new SqlCommand(
                @"SELECT CreatedAt FROM [Order] 
                  WHERE IdOrder = @IdOrder 
                    AND IdProduct = @IdProduct 
                    AND Amount = @Amount", connection, (SqlTransaction)transaction);

            orderCheckCmd.Parameters.AddWithValue("@IdOrder", request.IdOrder);
            orderCheckCmd.Parameters.AddWithValue("@IdProduct", request.IdProduct);
            orderCheckCmd.Parameters.AddWithValue("@Amount", request.Amount);

            var orderCreatedAt = await orderCheckCmd.ExecuteScalarAsync();
            if (orderCreatedAt is null || (DateTime)orderCreatedAt >= request.CreatedAt)
                return null;
            
            var fulfilledCheckCmd = new SqlCommand(
                "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder", connection, (SqlTransaction)transaction);
            fulfilledCheckCmd.Parameters.AddWithValue("@IdOrder", request.IdOrder);
            if (await fulfilledCheckCmd.ExecuteScalarAsync() is not null)
                return null;
            
            var priceCmd = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @IdProduct", connection, (SqlTransaction)transaction);
            priceCmd.Parameters.AddWithValue("@IdProduct", request.IdProduct);
            var unitPrice = (decimal)(await priceCmd.ExecuteScalarAsync() ?? 0);

            var totalPrice = unitPrice * request.Amount;
            var currentTime = DateTime.Now;

            var updateOrderCmd = new SqlCommand(
                "UPDATE [Order] SET FulfilledAt = @CurrentTime WHERE IdOrder = @IdOrder", connection, (SqlTransaction)transaction);
            updateOrderCmd.Parameters.AddWithValue("@CurrentTime", currentTime);
            updateOrderCmd.Parameters.AddWithValue("@IdOrder", request.IdOrder);
            await updateOrderCmd.ExecuteNonQueryAsync();

            var insertCmd = new SqlCommand(
                @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                  OUTPUT INSERTED.IdProductWarehouse
                  VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)", connection, (SqlTransaction)transaction);

            insertCmd.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
            insertCmd.Parameters.AddWithValue("@IdProduct", request.IdProduct);
            insertCmd.Parameters.AddWithValue("@IdOrder", request.IdOrder);
            insertCmd.Parameters.AddWithValue("@Amount", request.Amount);
            insertCmd.Parameters.AddWithValue("@Price", totalPrice);
            insertCmd.Parameters.AddWithValue("@CreatedAt", currentTime);

            var newId = (int?)await insertCmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
            return newId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw; 
        }
    }
}