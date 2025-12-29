using BasicWebApplicationCsharp.Domains;

namespace BasicWebApplicationCsharp.Tests
{
    public class DomainClassesTests
    {
        [Fact]
        public void OrderStatus_Should_Have_Expected_Values()
        {
            Assert.Equal(0, (int)OrderStatus.Draft);
            Assert.Equal(1, (int)OrderStatus.Placed);
            Assert.Equal(2, (int)OrderStatus.Paid);
            Assert.Equal(3, (int)OrderStatus.Shipped);
            Assert.Equal(4, (int)OrderStatus.Delivered);
            Assert.Equal(5, (int)OrderStatus.Completed);
            Assert.Equal(6, (int)OrderStatus.Cancelled);
        }

        [Fact]
        public void UserRole_Should_Have_Expected_Values()
        {
            Assert.Equal(0, (int)UserRole.Customer);
            Assert.Equal(1, (int)UserRole.Manager);
            Assert.Equal(2, (int)UserRole.Admin);
        }

        [Fact]
        public void Order_Constructor_Should_Set_All_Properties()
        {
            var items = new List<OrderItem>
                {
                    new OrderItem(1, 2, 5m),
                    new OrderItem(2, 1, 10m)
                };

            var createdAt = DateTime.UtcNow;

            var order = new Order(
                1,
                42,
                OrderStatus.Placed,
                createdAt,
                items
            );

            Assert.Equal(1, order.Id);
            Assert.Equal(42, order.UserId);
            Assert.Equal(OrderStatus.Placed, order.Status);
            Assert.Equal(createdAt, order.CreatedAt);
            Assert.Equal(items, order.Items);
        }

        [Fact]
        public void Order_Constructor_Should_Calculate_TotalPrice()
        {
            var items = new List<OrderItem>
                {
                    new OrderItem(1, 2, 5m),   // 10
                    new OrderItem(2, 3, 4m)    // 12
                };

            var order = new Order(
                1,
                1,
                OrderStatus.Draft,
                DateTime.UtcNow,
                items
            );

            Assert.Equal(22m, order.TotalPrice);
        }

        [Fact]
        public void Order_Constructor_Should_Set_TotalPrice_To_Zero_When_No_Items()
        {
            var order = new Order(
                1,
                1,
                OrderStatus.Draft,
                DateTime.UtcNow,
                new List<OrderItem>()
            );

            Assert.Equal(0m, order.TotalPrice);
        }


        [Fact]
        public void OrderItem_Constructor_Should_Set_Properties()
        {
            var item = new OrderItem(1, 2, 9.99m);

            Assert.Equal(1, item.ProductId);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(9.99m, item.UnitPrice);
        }

        [Fact]
        public void OrderItem_Constructor_Should_Throw_When_Quantity_Is_Zero_Or_Negative()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new OrderItem(1, 0, 9.99m)
            );

            Assert.Equal("quantity", ex.ParamName);
        }

        [Fact]
        public void OrderItem_Constructor_Should_Throw_When_Price_Is_Negative()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new OrderItem(1, 1, -1m)
            );

            Assert.Equal("unitPrice", ex.ParamName);
        }

        [Fact]
        public void Product_Constructor_Should_Set_Properties()
        {
            var product = new Product(
                1,
                "Cola",
                "Drink",
                "SKU-1",
                5.99m,
                10
            );

            Assert.Equal(1, product.Id);
            Assert.Equal("Cola", product.Name);
            Assert.Equal("Drink", product.Description);
            Assert.Equal("SKU-1", product.SKU);
            Assert.Equal(5.99m, product.Price);
            Assert.Equal(10, product.StockQuantity);
        }

        [Fact]
        public void Product_Constructor_Should_Throw_When_Price_Is_Negative()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Product(1, "X", "Y", "Z", -1m, 10)
            );

            Assert.Equal("price", ex.ParamName);
        }

        [Fact]
        public void Product_Constructor_Should_Throw_When_StockQuantity_Is_Negative()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Product(1, "X", "Y", "Z", 1m, -5)
            );

            Assert.Equal("stockQuantity", ex.ParamName);
        }

        [Fact]
        public void User_Constructor_Should_Set_All_Properties()
        {
            var user = new User(
                1,
                "Alice",
                "alice@email.com",
                "hash",
                UserRole.Admin
            );

            Assert.Equal(1, user.Id);
            Assert.Equal("Alice", user.Username);
            Assert.Equal("alice@email.com", user.Email);
            Assert.Equal("hash", user.PasswordHash);
            Assert.Equal(UserRole.Admin, user.Role);
        }
    }
}
