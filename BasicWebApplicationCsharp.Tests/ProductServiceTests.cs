using BasicWebApplicationCsharp.Domains;
using BasicWebApplicationCsharp.Entities;
using BasicWebApplicationCsharp.Services;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApplicationCsharp.Tests
{
    public class ProductServiceTests
    {
        private readonly AppDbContext _context;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _context = new AppDbContext(options);
            _service = new ProductService(_context);
        }

        [Fact]
        public void Create_Should_Add_Product_To_Db()
        {
            var product = _service.Create(
                "Cola",
                "Sweet drink",
                "SKU-001",
                5.99m,
                10
            );

            Assert.NotNull(product);
            Assert.Equal("Cola", product.Name);
            Assert.Equal(1, _context.Products.Count());
        }

        [Fact]
        public void Create_Should_Persist_All_Fields()
        {
            _service.Create("Cola", "Sweet drink", "SKU-001", 5.99m, 10);

            var entity = _context.Products.Single();

            Assert.Equal("Cola", entity.Name);
            Assert.Equal("Sweet drink", entity.Description);
            Assert.Equal("SKU-001", entity.Sku);
            Assert.Equal(5.99m, entity.Price);
            Assert.Equal(10, entity.StockQuantity);
        }

        [Fact]
        public void GetAll_Should_Return_All_Products()
        {
            _service.Create("Cola", "Drink", "SKU-1", 5m, 10);
            _service.Create("Water", "Drink", "SKU-2", 2m, 20);

            var products = _service.GetAll();

            Assert.Equal(2, products.Count);
            Assert.Contains(products, p => p.Name == "Cola");
            Assert.Contains(products, p => p.Name == "Water");
        }

        [Fact]
        public void GetAll_Should_Return_Empty_List_When_No_Products()
        {
            var products = _service.GetAll();

            Assert.NotNull(products);
            Assert.Empty(products);
        }

        [Fact]
        public void GetById_Should_Return_Product_When_Exists()
        {
            var created = _service.Create("Cola", "Drink", "SKU-1", 5m, 10);

            var product = _service.GetById(created.Id);

            Assert.NotNull(product);
            Assert.Equal(created.Id, product.Id);
            Assert.Equal("Cola", product.Name);
        }

        [Fact]
        public void GetById_Should_Return_Null_When_Not_Exists()
        {
            var product = _service.GetById(999);

            Assert.Null(product);
        }

        [Fact]
        public void Update_Should_Modify_Product_When_Exists()
        {
            var created = _service.Create("Cola", "Drink", "SKU-1", 5m, 10);

            var updated = _service.Update(
                created.Id,
                "Cola Zero",
                "No sugar",
                "SKU-1Z",
                6m,
                15
            );

            Assert.NotNull(updated);
            Assert.Equal("Cola Zero", updated.Name);
            Assert.Equal("No sugar", updated.Description);
            Assert.Equal("SKU-1Z", updated.SKU);
            Assert.Equal(6m, updated.Price);
            Assert.Equal(15, updated.StockQuantity);
        }

        [Fact]
        public void Update_Should_Return_Null_When_Product_Not_Found()
        {
            var result = _service.Update(
                123,
                "X",
                "Y",
                "Z",
                1m,
                1
            );

            Assert.Null(result);
        }

        [Fact]
        public void Delete_Should_Remove_Product_When_Exists()
        {
            var created = _service.Create("Cola", "Drink", "SKU-1", 5m, 10);

            var result = _service.Delete(created.Id);

            Assert.True(result);
            Assert.Empty(_context.Products);
        }

        [Fact]
        public void Delete_Should_Return_False_When_Product_Not_Found()
        {
            var result = _service.Delete(999);

            Assert.False(result);
        }

        [Fact]
        public void EntityFromDomain_Should_Map_All_Fields()
        {
            var domain = new Product(
                1,
                "Cola",
                "Drink",
                "SKU-1",
                5m,
                10
            );

            var entity = _service.EntityFromDomain(domain);

            Assert.Equal(1, entity.Id);
            Assert.Equal("Cola", entity.Name);
            Assert.Equal("Drink", entity.Description);
            Assert.Equal("SKU-1", entity.Sku);
            Assert.Equal(5m, entity.Price);
            Assert.Equal(10, entity.StockQuantity);
        }

        [Fact]
        public void DomainFromEntity_Should_Map_All_Fields()
        {
            var entity = new ProductEntity
            {
                Id = 1,
                Name = "Cola",
                Description = "Drink",
                Sku = "SKU-1",
                Price = 5m,
                StockQuantity = 10
            };

            var domain = _service.DomainFromEntity(entity);

            Assert.Equal(1, domain.Id);
            Assert.Equal("Cola", domain.Name);
            Assert.Equal("Drink", domain.Description);
            Assert.Equal("SKU-1", domain.SKU);
            Assert.Equal(5m, domain.Price);
            Assert.Equal(10, domain.StockQuantity);
        }
    }

}
