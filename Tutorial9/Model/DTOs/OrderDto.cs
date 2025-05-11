using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutorial9.Model.DTOs;

public class OrderDto
{
    [Key]
    public int Id { get; set; }

    public int IdProduct { get; set; }

    [ForeignKey("IdProduct")] public ProductDto Product { get; set; } = null!;

    public int Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? FulfilledAt { get; set; }
}