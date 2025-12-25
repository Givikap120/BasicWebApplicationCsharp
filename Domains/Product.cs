namespace BasicWebApplicationCsharp.Domains
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = "";
        public string Description { get; private set; } = "";
        public string SKU { get; private set; } = "";
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }

        public Product(int id, string name, string description, string sku, decimal price, int stockQuantity)
        {
            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

            Id = id;
            Name = name;
            Description = description;
            SKU = sku;
            Price = price;
            StockQuantity = stockQuantity;
        }
    }
}
