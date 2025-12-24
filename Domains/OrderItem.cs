namespace BasicWebApplicationCsharp.Domains
{
    public class OrderItem
    {
        public int ProductId { get; private set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; private set; }

        public OrderItem(int productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));

            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
