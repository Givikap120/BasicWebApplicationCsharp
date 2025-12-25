
using BasicWebApplicationCsharp.Domains;
using BasicWebApplicationCsharp.Entities;

namespace BasicWebApplicationCsharp.Services
{
    public class ProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public ProductEntity EntityFromDomain(Product product)
        {
            return new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Sku = product.SKU,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }

        public Product DomainFromEntity(ProductEntity product)
        {
            return new Product(product.Id, product.Name, product.Description, product.Sku, product.Price, product.StockQuantity);
        }

        public List<Product> GetAll()
        {
            var productEntities = _db.Products.ToList();
            return productEntities.Select(DomainFromEntity).ToList();
        }

        public Product? GetById(int id)
        {
            var productEntity = _db.Products.Find(id);
            return productEntity == null ? null : DomainFromEntity(productEntity);
        }

        public Product Create(string name, string description, string sku, decimal price, int stockQuantity)
        {
            var product = new ProductEntity
            {
                Name = name,
                Description = description,
                Sku = sku,
                Price = price,
                StockQuantity = stockQuantity
            };

            _db.Products.Add(product);
            _db.SaveChanges();

            return DomainFromEntity(product);
        }

        public bool Delete(int id)
        {
            var entity = _db.Products.Find(id);
            if (entity == null) return false;

            _db.Products.Remove(entity);
            _db.SaveChanges();

            return true;
        }

        public Product? Update(int id, string name, string description, string sku, decimal price, int stockQuantity)
        {
            var entity = _db.Products.Find(id);
            if (entity == null) return null;

            entity.Name = name;
            entity.Description = description;
            entity.Sku = sku;
            entity.Price = price;
            entity.StockQuantity = stockQuantity;

            _db.SaveChanges();
            return DomainFromEntity(entity);
        }
    }
}
