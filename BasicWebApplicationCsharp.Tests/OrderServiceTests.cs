using BasicWebApplicationCsharp.Domains;
using BasicWebApplicationCsharp.Entities;
using BasicWebApplicationCsharp.Services;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApplicationCsharp.Tests
{
    public class OrderServiceTests
    {
        private readonly AppDbContext _context;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _context = new AppDbContext(options);
            _service = new OrderService(_context);
        }

        private ProductEntity AddProduct(int id, int stock = 10, decimal price = 5m)
        {
            var product = new ProductEntity
            {
                Id = id,
                Name = $"Product{id}",
                Description = "Test",
                Sku = $"SKU-{id}",
                Price = price,
                StockQuantity = stock
            };

            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        private OrderEntity AddDraftOrder(int userId = 1)
        {
            var order = new OrderEntity
            {
                UserId = userId,
                Status = (int)OrderStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            _context.SaveChanges();
            return order;
        }

        [Fact]
        public void Create_Should_Create_Draft_Order()
        {
            var order = _service.Create(1);

            Assert.NotNull(order);
            Assert.Equal(OrderStatus.Draft, order.Status);
            Assert.Equal(1, order.UserId);
        }

        [Fact]
        public void GetById_Should_Return_Order_With_Items()
        {
            var orderEntity = AddDraftOrder();
            AddProduct(1);

            _service.AddItem(orderEntity.Id, 1, 2);

            var order = _service.GetById(orderEntity.Id);

            Assert.NotNull(order);
            Assert.Single(order.Items);
            Assert.Equal(2, order.Items[0].Quantity);
        }

        [Fact]
        public void GetById_Should_Return_Null_When_Not_Found()
        {
            var order = _service.GetById(999);
            Assert.Null(order);
        }

        [Fact]
        public void AddItem_Should_Add_New_Item_And_Decrease_Stock()
        {
            var order = AddDraftOrder();
            var product = AddProduct(1, stock: 10);

            var result = _service.AddItem(order.Id, product.Id, 3);

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(3, result.Items[0].Quantity);

            var updatedProduct = _context.Products.Single();
            Assert.Equal(7, updatedProduct.StockQuantity);
        }

        [Fact]
        public void AddItem_Should_Increase_Quantity_When_Item_Exists()
        {
            var order = AddDraftOrder();
            AddProduct(1, stock: 10);

            _service.AddItem(order.Id, 1, 2);
            var result = _service.AddItem(order.Id, 1, 3);

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(5, result.Items[0].Quantity);
        }

        [Fact]
        public void AddItem_Should_Return_Null_When_Not_Draft()
        {
            var order = AddDraftOrder();
            order.Status = (int)OrderStatus.Placed;
            _context.SaveChanges();

            AddProduct(1);

            var result = _service.AddItem(order.Id, 1, 1);

            Assert.Null(result);
        }

        [Fact]
        public void AddItem_Should_Return_Null_When_Quantity_Invalid()
        {
            var order = AddDraftOrder();
            AddProduct(1);

            var result = _service.AddItem(order.Id, 1, 0);

            Assert.Null(result);
        }

        [Fact]
        public void ChangeItemQuantity_Should_Adjust_Stock_Correctly()
        {
            var order = AddDraftOrder();
            var product = AddProduct(1, stock: 10);

            _service.AddItem(order.Id, 1, 3);

            var result = _service.ChangeItemQuantity(order.Id, 1, 5);

            Assert.NotNull(result);
            Assert.Equal(5, result.Items.Single().Quantity);

            var updatedProduct = _context.Products.Single();
            Assert.Equal(5, updatedProduct.StockQuantity);
        }

        [Fact]
        public void ChangeItemQuantity_Should_Return_Null_When_Item_Not_Found()
        {
            var order = AddDraftOrder();
            AddProduct(1);

            var result = _service.ChangeItemQuantity(order.Id, 1, 2);

            Assert.Null(result);
        }

        [Fact]
        public void RemoveItem_Should_Remove_Item_And_Restore_Stock()
        {
            var order = AddDraftOrder();
            var product = AddProduct(1, stock: 10);

            _service.AddItem(order.Id, 1, 4);

            var result = _service.RemoveItem(order.Id, 1);

            Assert.NotNull(result);
            Assert.Empty(result.Items);

            var updatedProduct = _context.Products.Single();
            Assert.Equal(10, updatedProduct.StockQuantity);
        }

        [Fact]
        public void RemoveItem_Should_Return_Null_When_Item_Not_Found()
        {
            var order = AddDraftOrder();
            AddProduct(1);

            var result = _service.RemoveItem(order.Id, 1);

            Assert.Null(result);
        }

        [Fact]
        public void Place_Should_Work_Only_From_Draft()
        {
            var order = AddDraftOrder();

            var placed = _service.Place(order.Id);

            Assert.NotNull(placed);
            Assert.Equal(OrderStatus.Placed, placed.Status);
        }

        [Fact]
        public void Pay_Should_Work_Only_From_Placed()
        {
            var order = AddDraftOrder();
            _service.Place(order.Id);

            var paid = _service.Pay(order.Id);

            Assert.NotNull(paid);
            Assert.Equal(OrderStatus.Paid, paid.Status);
        }

        [Fact]
        public void ConfirmPickup_Should_Work_Only_From_Paid()
        {
            var order = AddDraftOrder();
            _service.Place(order.Id);
            _service.Pay(order.Id);

            var shipped = _service.ConfirmPickup(order.Id);

            Assert.NotNull(shipped);
            Assert.Equal(OrderStatus.Shipped, shipped.Status);
        }

        [Fact]
        public void Complete_Should_Work_Only_From_Delivered()
        {
            var order = AddDraftOrder();
            _service.Place(order.Id);
            _service.Pay(order.Id);
            _service.ConfirmPickup(order.Id);
            _service.ConfirmDelivery(order.Id);

            var completed = _service.Complete(order.Id);

            Assert.NotNull(completed);
            Assert.Equal(OrderStatus.Completed, completed.Status);
        }

        [Fact]
        public void Place_Should_Return_Null_When_Not_Draft()
        {
            var order = AddDraftOrder();
            order.Status = (int)OrderStatus.Placed;
            _context.SaveChanges();

            var result = _service.Place(order.Id);

            Assert.Null(result);
        }

        [Fact]
        public void Pay_Should_Return_Null_When_Not_Placed()
        {
            var order = AddDraftOrder(); // still Draft

            var result = _service.Pay(order.Id);

            Assert.Null(result);
        }

        [Fact]
        public void ConfirmPickup_Should_Return_Null_When_Not_Paid()
        {
            var order = AddDraftOrder();
            _service.Place(order.Id); // Placed, not Paid

            var result = _service.ConfirmPickup(order.Id);

            Assert.Null(result);
        }

        [Fact]
        public void ConfirmDelivery_Should_Return_Null_When_Not_Shipped()
        {
            var order = AddDraftOrder();
            _service.Place(order.Id);
            _service.Pay(order.Id); // Paid, not Shipped

            var result = _service.ConfirmDelivery(order.Id);

            Assert.Null(result);
        }

        [Fact]
        public void Complete_Should_Return_Null_When_Not_Delivered()
        {
            var order = AddDraftOrder();
            _service.Place(order.Id);
            _service.Pay(order.Id);
            _service.ConfirmPickup(order.Id); // Shipped, not Delivered

            var result = _service.Complete(order.Id);

            Assert.Null(result);
        }


        [Fact]
        public void Cancel_Should_Work_Unless_Completed()
        {
            var order = AddDraftOrder();

            var cancelled = _service.Cancel(order.Id);

            Assert.NotNull(cancelled);
            Assert.Equal(OrderStatus.Cancelled, cancelled.Status);
        }

        [Fact]
        public void Cancel_Should_Return_Null_When_Completed()
        {
            var order = AddDraftOrder();
            _service.Place(order.Id);
            _service.Pay(order.Id);
            _service.ConfirmPickup(order.Id);
            _service.ConfirmDelivery(order.Id);
            _service.Complete(order.Id);

            var result = _service.Cancel(order.Id);

            Assert.Null(result);
        }

        [Fact]
        public void Cancel_Should_Return_Null_When_Order_Not_Found()
        {
            var result = _service.Cancel(999);

            Assert.Null(result);
        }

    }
}
