using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApplicationCsharp.Entities;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<OrderEntity> Orders { get; set; }

    public virtual DbSet<OrderItemEntity> OrderItems { get; set; }

    public virtual DbSet<ProductEntity> Products { get; set; }

    public virtual DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__Id");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_User");
        });

        modelBuilder.Entity<OrderItemEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderItems_Id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK_OrderItems_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Product");
        });

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__Id");
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
