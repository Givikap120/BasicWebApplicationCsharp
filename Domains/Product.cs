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
    }
}
