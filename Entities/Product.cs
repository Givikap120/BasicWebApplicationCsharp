using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApplicationCsharp.Entities;

[Index("Sku", Name = "UQ__Products__SKU", IsUnique = true)]
public partial class ProductEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Column("SKU")]
    [StringLength(100)]
    public string Sku { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
}
