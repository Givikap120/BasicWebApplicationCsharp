using BasicWebApplicationCsharp.Domains;
using BasicWebApplicationCsharp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApplicationCsharp.Services
{
    public class OrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }

        private Order DomainFromEntity(OrderEntity entity)
        {
            return new Order(
                entity.Id,
                entity.UserId,
                (OrderStatus)entity.Status,
                entity.CreatedAt,
                entity.OrderItems.Select(i => new OrderItem(i.ProductId, i.Quantity, i.UnitPrice)).ToList()
            );
        }

        private OrderEntity EntityFromDomain(Order domain)
        {
            var entity = new OrderEntity
            {
                Id = domain.Id,
                UserId = domain.UserId,
                Status = (int)domain.Status,
                CreatedAt = domain.CreatedAt,
                OrderItems = domain.Items.Select(i => new OrderItemEntity
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
            return entity;
        }

        public Order? GetById(int id)
        {
            var entity = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == id);
            return entity == null ? null : DomainFromEntity(entity);
        }

        public Order Create(int userId)
        {
            var entity = new OrderEntity
            {
                UserId = userId,
                Status = (int)OrderStatus.Draft
            };

            _db.Orders.Add(entity);
            _db.SaveChanges();

            return DomainFromEntity(entity);
        }

        public Order? AddItem(int orderId, int productId, int quantity)
        {
            if (quantity <= 0) return null;

            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status != (int)OrderStatus.Draft) return null;

            var productExists = _db.Products.Any(p => p.Id == productId);
            if (!productExists) return null;

            var existingItemEntity = order.OrderItems.FirstOrDefault(i => i.ProductId == productId);

            if (existingItemEntity != null)
            {
                existingItemEntity.Quantity += quantity;
            }
            else
            {
                order.OrderItems.Add(new OrderItemEntity
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            _db.SaveChanges();
            return DomainFromEntity(order);
        }

        public Order? ChangeItemQuantity(int orderId, int productId, int quantity)
        {
            if (quantity <= 0) return null;

            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status != (int)OrderStatus.Draft) return null;

            var item = order.OrderItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return null;

            item.Quantity = quantity;
            _db.SaveChanges();

            return DomainFromEntity(order);
        }

        public Order? RemoveItem(int orderId, int productId)
        {
            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status != (int)OrderStatus.Draft) return null;

            var item = order.OrderItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return null;

            order.OrderItems.Remove(item);
            _db.SaveChanges();

            return DomainFromEntity(order);
        }

        public Order? Place(int orderId)
        {
            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status != (int)OrderStatus.Draft) return null;

            order.Status = (int)OrderStatus.Placed;
            _db.SaveChanges();

            return DomainFromEntity(order);
        }

        public Order? Pay(int orderId)
        {
            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status != (int)OrderStatus.Placed) return null;

            order.Status = (int)OrderStatus.Paid;
            _db.SaveChanges();

            return DomainFromEntity(order);
        }

        public Order? ConfirmPickup(int orderId)
        {
            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status != (int)OrderStatus.Paid)
                return null;

            order.Status = (int)OrderStatus.Shipped;
            _db.SaveChanges();

            return DomainFromEntity(order);
        }

        public Order? ConfirmDelivery(int orderId)
        {
            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status != (int)OrderStatus.Shipped)
                return null;

            order.Status = (int)OrderStatus.Delivered;
            _db.SaveChanges();

            return DomainFromEntity(order);
        }

        public Order? Complete(int orderId)
        {
            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status != (int)OrderStatus.Delivered)
                return null;

            order.Status = (int)OrderStatus.Completed;
            _db.SaveChanges();

            return DomainFromEntity(order);
        }

        public Order? Cancel(int orderId)
        {
            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status == (int)OrderStatus.Completed)
                return null;

            order.Status = (int)OrderStatus.Cancelled;
            _db.SaveChanges();

            return DomainFromEntity(order);
        }
    }

}
