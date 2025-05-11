using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutorial9.Model.DTOs;

public class ProductDto
{
    [Key] public int Id { get; set; }
    [MaxLength(200)] public string Name { get; set; } = string.Empty;
    [MaxLength(200)] public string Description { get; set; } = string.Empty;
    /*[Precision(18, 2)]*/ public decimal Price { get; set; }
}