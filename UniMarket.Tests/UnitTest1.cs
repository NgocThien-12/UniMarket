using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;
using UniMarket.ViewModels;
using Xunit;

namespace UniMarket.Tests
{
    public class UniMarketBusinessLogicTests
    {
        private ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void CartViewModel_Calculates_TotalAmount_And_Quantity_Correctly()
        {
            // Arrange
            var cart = new CartViewModel
            {
                Items = new List<CartItemViewModel>
                {
                    new CartItemViewModel { ProductId = 1, ProductName = "Sách", UnitPrice = 50000, Quantity = 2 },
                    new CartItemViewModel { ProductId = 2, ProductName = "Balo", UnitPrice = 200000, Quantity = 1 }
                }
            };

            // Act & Assert
            Assert.Equal(3, cart.TotalQuantity);
            Assert.Equal(300000, cart.TotalAmount); // 50000*2 + 200000*1 = 300,000
        }

        [Fact]
        public void CartItemViewModel_Calculates_Subtotal_Correctly()
        {
            // Arrange
            var item = new CartItemViewModel
            {
                ProductId = 1,
                ProductName = "Laptop Dell",
                UnitPrice = 4500000,
                Quantity = 2
            };

            // Act & Assert
            Assert.Equal(9000000, item.Subtotal);
        }

        [Fact]
        public void Product_CompatibilityProperties_Map_Correctly()
        {
            // Arrange
            var product = new Product
            {
                Title = "Xe đạp địa hình",
                OwnerId = "user_123",
                Price = 1200000,
                Status = ProductStatus.Approved
            };

            // Assert
            Assert.Equal("Xe đạp địa hình", product.Name);
            Assert.Equal("user_123", product.SellerId);
            Assert.True(product.IsAvailable);
            Assert.Equal(1, product.AvailableQuantity);

            // Change status to Sold
            product.Status = ProductStatus.Sold;
            Assert.False(product.IsAvailable);
            Assert.Equal(0, product.AvailableQuantity);
        }

        [Fact]
        public void Order_CompatibilityProperties_Map_Correctly()
        {
            // Arrange
            var order = new Order
            {
                OrderCode = "UM2610131234",
                FullName = "Nguyễn Văn Nam",
                ShippingAddress = "Ký túc xá B",
                Note = "Giao giờ trưa",
                TotalAmount = 500000,
                Status = OrderStatus.Pending
            };

            // Assert
            Assert.Equal("UM2610131234", order.OrderNumber);
            Assert.Equal("Nguyễn Văn Nam", order.RecipientName);
            Assert.Equal("Ký túc xá B", order.DeliveryAddress);
            Assert.Equal("Giao giờ trưa", order.Notes);
        }

        [Fact]
        public async Task Database_Can_Query_Approved_Products_In_Category()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            var category = new Category { Id = 1, Name = "Sách", Slug = "sach", DisplayOrder = 1 };
            context.Categories.Add(category);

            var approvedProduct = new Product
            {
                Id = 1,
                Title = "Giáo trình C#",
                Description = "Sách hay",
                Price = 80000,
                CategoryId = 1,
                OwnerId = "u1",
                Status = ProductStatus.Approved
            };

            var pendingProduct = new Product
            {
                Id = 2,
                Title = "Giáo trình Java",
                Description = "Đang chờ duyệt",
                Price = 90000,
                CategoryId = 1,
                OwnerId = "u2",
                Status = ProductStatus.Pending
            };

            context.Products.AddRange(approvedProduct, pendingProduct);
            await context.SaveChangesAsync();

            // Act
            var publicProducts = await context.Products
                .Where(p => p.CategoryId == 1 && p.Status == ProductStatus.Approved)
                .ToListAsync();

            // Assert
            Assert.Single(publicProducts);
            Assert.Equal("Giáo trình C#", publicProducts[0].Title);
        }

        [Fact]
        public void ProductFilterViewModel_Calculates_TotalPages_Accurately()
        {
            // Arrange
            var filter = new ProductFilterViewModel
            {
                PageSize = 9,
                TotalItems = 20
            };

            // Act & Assert (20 / 9 = 2.22 -> 3 pages)
            Assert.Equal(3, filter.TotalPages);
        }
    }
}
