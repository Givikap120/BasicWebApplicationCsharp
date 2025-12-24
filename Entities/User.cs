using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApplicationCsharp.Entities;

[Index("Email", Name = "UQ__Users__A9D10534D25E80B8", IsUnique = true)]
public partial class UserEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    public int Role { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
}
