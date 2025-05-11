namespace Tutorial9.Model.DTOs;

public class NewProductToWarehouseDto
{
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    public int IdOrder { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
