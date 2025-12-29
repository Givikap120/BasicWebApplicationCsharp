using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApplicationCsharp.Entities;

public partial class OrderEntity
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPrice { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual UserEntity User { get; set; } = null!;
}
