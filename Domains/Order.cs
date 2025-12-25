namespace BasicWebApplicationCsharp.Domains
{
    public enum OrderStatus
    {
        Draft,
        Placed,
        Paid,
        Shipped,
        Delivered,
        Completed,
        Cancelled
    }

    public class Order
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Draft;
        public DateTime CreatedAt { get; private set; }
        public List<OrderItem> Items { get; private set; } = [];
        public decimal TotalPrice { get; private set; }

        public Order(int id, int userId, OrderStatus status, DateTime createdAt, List<OrderItem> items)
        {
            Id = id;
            UserId = userId;
            Status = status;
            CreatedAt = createdAt;
            Items = items;
            TotalPrice = Items.Sum(i => i.UnitPrice * i.Quantity);
        }
    }
}
