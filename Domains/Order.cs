namespace BasicWebApplicationCsharp.Domains
{
    public enum OrderStatus
    {
        Pending,
        Paid,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; private set; }
        public decimal TotalPrice { get; private set; }
    }
}
