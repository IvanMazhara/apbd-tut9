using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutorial9.Model.DTOs;

public class ProductWarehouseDto
{
    [Key] public int Id { get; set; }
    
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    public int IdOrder { get; set; }
    
    [ForeignKey("IdProduct")] public ProductDto Product { get; set; } = null!;
    [ForeignKey("IdWarehouse")] public WarehouseDto Warehouse { get; set; } = null!;
    [ForeignKey("IdOrder")] public OrderDto Order { get; set; } = null!;
    
    public int Amount { get; set; }
    [Column(TypeName = "decimal(25,2)")] public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}