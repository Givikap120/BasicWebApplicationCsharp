namespace BasicWebApplicationCsharp.Domains
{
    public class OrderItem
    {
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public OrderItem(int productId, int quantity, decimal unitPrice)
        {
            if (unitPrice < 0)
                throw new ArgumentException("Price cannot be negative", nameof(unitPrice));

            if (quantity <= 0)
                throw new ArgumentException("Quantity cannot be zero or negative", nameof(quantity));

            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
