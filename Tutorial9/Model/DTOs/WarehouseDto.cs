using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model.DTOs;

public class WarehouseDto
{
    [Key] public int Id { get; set; }
    [MaxLength(200)] public string Name { get; set; }
    [MaxLength(200)] public string Description { get; set; }
}