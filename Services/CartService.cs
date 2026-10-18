using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;
using UniMarket.ViewModels;

namespace UniMarket.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionCartKey = "UniMarket_SessionCart";

        public CartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public async Task<CartViewModel> GetCartAsync(string? userId)
        {
            var cartVm = new CartViewModel();

            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Owner)
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Images)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    foreach (var item in cart.Items.Where(i => i.Product != null))
                    {
                        var mainImg = item.Product!.Images.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
                                      ?? item.Product.Images.FirstOrDefault()?.ImageUrl
                                      ?? "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=480&h=360&fit=crop&auto=format";

                        cartVm.Items.Add(new CartItemViewModel
                        {
                            ProductId = item.ProductId,
                            ProductName = item.Product.Title,
                            UnitPrice = item.Product.Price,
                            Quantity = Math.Max(1, item.Quantity),
                            MaxQuantity = item.Product.Status == ProductStatus.Approved ? 10 : 0,
                            ImageUrl = mainImg,
                            SellerName = item.Product.Owner?.FullName ?? "Người bán UniMarket"
                        });
                    }
                }
            }
            else
            {
                var sessionCart = GetSessionCart();
                if (sessionCart.Any())
                {
                    var productIds = sessionCart.Keys.ToList();
                    var products = await _context.Products
                        .Include(p => p.Owner)
                        .Include(p => p.Images)
                        .Where(p => productIds.Contains(p.Id) && p.Status == ProductStatus.Approved)
                        .ToListAsync();

                    foreach (var product in products)
                    {
                        var qty = sessionCart[product.Id];
                        var mainImg = product.Images.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
                                      ?? product.Images.FirstOrDefault()?.ImageUrl
                                      ?? "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=480&h=360&fit=crop&auto=format";

                        cartVm.Items.Add(new CartItemViewModel
                        {
                            ProductId = product.Id,
                            ProductName = product.Title,
                            UnitPrice = product.Price,
                            Quantity = Math.Max(1, qty),
                            MaxQuantity = product.Status == ProductStatus.Approved ? 10 : 0,
                            ImageUrl = mainImg,
                            SellerName = product.Owner?.FullName ?? "Người bán UniMarket"
                        });
                    }
                }
            }

            return cartVm;
        }

        public async Task<(bool Success, string Message)> AddToCartAsync(string? userId, int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.Status != ProductStatus.Approved)
            {
                return (false, "Sản phẩm hiện không khả dụng hoặc đã bán.");
            }

            if (!string.IsNullOrEmpty(userId) && product.OwnerId == userId)
            {
                return (false, "Bạn không thể tự mua sản phẩm do chính mình đăng bán.");
            }

            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart { UserId = userId, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    item.Quantity += quantity;
                }
                else
                {
                    cart.Items.Add(new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = quantity,
                        AddedAt = DateTimeOffset.UtcNow
                    });
                }

                cart.UpdatedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
            }
            else
            {
                var sessionCart = GetSessionCart();
                int currentQty = sessionCart.ContainsKey(productId) ? sessionCart[productId] : 0;
                sessionCart[productId] = currentQty + quantity;
                SaveSessionCart(sessionCart);
            }

            return (true, "Đã thêm sản phẩm vào giỏ hàng thành công!");
        }

        public async Task<(bool Success, string Message)> UpdateQuantityAsync(string? userId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                await RemoveFromCartAsync(userId, productId);
                return (true, "Đã xóa sản phẩm khỏi giỏ hàng.");
            }

            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                    if (item != null)
                    {
                        item.Quantity = quantity;
                        cart.UpdatedAt = DateTimeOffset.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                var sessionCart = GetSessionCart();
                if (sessionCart.ContainsKey(productId))
                {
                    sessionCart[productId] = quantity;
                    SaveSessionCart(sessionCart);
                }
            }

            return (true, "Đã cập nhật số lượng thành công.");
        }

        public async Task<bool> RemoveFromCartAsync(string? userId, int productId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                    if (item != null)
                    {
                        _context.CartItems.Remove(item);
                        cart.UpdatedAt = DateTimeOffset.UtcNow;
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
            }
            else
            {
                var sessionCart = GetSessionCart();
                if (sessionCart.Remove(productId))
                {
                    SaveSessionCart(sessionCart);
                    return true;
                }
            }

            return false;
        }

        public async Task ClearCartAsync(string? userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    _context.CartItems.RemoveRange(cart.Items);
                    cart.UpdatedAt = DateTimeOffset.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                Session.Remove(SessionCartKey);
            }
        }

        public async Task<int> GetCartItemCountAsync(string? userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                return cart?.Items.Sum(i => i.Quantity) ?? 0;
            }
            else
            {
                var sessionCart = GetSessionCart();
                return sessionCart.Values.Sum();
            }
        }

        public async Task<(bool Success, string Message, Order? Order)> CheckoutAsync(string userId, CheckoutViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Images)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.Items.Any())
                {
                    return (false, "Giỏ hàng của bạn đang trống.", null);
                }

                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in cart.Items)
                {
                    var product = item.Product;
                    if (product == null || product.Status != ProductStatus.Approved)
                    {
                        await transaction.RollbackAsync();
                        var prodTitle = product?.Title ?? $"ID #{item.ProductId}";
                        return (false, $"Sản phẩm \"{prodTitle}\" hiện không còn khả dụng hoặc đã được bán. Vui lòng quay lại giỏ hàng và xóa sản phẩm này trước khi đặt hàng.", null);
                    }

                    if (!string.IsNullOrEmpty(userId) && product.OwnerId == userId)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Bạn không thể tự đặt mua sản phẩm \"{product.Title}\" do chính bạn đăng bán.", null);
                    }

                    var itemSubtotal = product.Price * item.Quantity;
                    totalAmount += itemSubtotal;

                    var mainImg = product.Images.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
                                  ?? product.Images.FirstOrDefault()?.ImageUrl;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        ProductTitle = product.Title,
                        ProductPrice = product.Price,
                        Quantity = item.Quantity,
                        SellerId = product.OwnerId,
                        ProductImageUrl = mainImg
                    });

                    // Đánh dấu sản phẩm đã bán nếu là đồ cũ duy nhất
                    product.Status = ProductStatus.Sold;
                    product.UpdatedAt = DateTimeOffset.UtcNow;
                }

                var orderCode = $"UM{DateTime.UtcNow:yyMMdd}{new Random().Next(1000, 9999)}";

                        var order = new Order
                        {
                            OrderCode = orderCode,
                            BuyerId = userId,
                            FullName = model.RecipientName.Trim(),
                            PhoneNumber = model.PhoneNumber.Trim(),
                            ShippingAddress = model.DeliveryAddress.Trim(),

                            // Tạm dùng địa chỉ giao hàng để tránh City bị NULL
                            City = model.DeliveryAddress.Trim(),

                            Note = model.Notes?.Trim(),
                            ShippingMethod = 0,
                            ShippingFee = 0,
                            PaymentMethod = 0,
                            PaymentStatus = 0,
                            TotalAmount = totalAmount,
                            Status = OrderStatus.Pending,
                            CreatedAt = DateTimeOffset.UtcNow,
                            UpdatedAt = DateTimeOffset.UtcNow,
                            Items = orderItems
                        };
                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cart.Items);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Đặt hàng thành công!", order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Có lỗi xảy ra khi tạo đơn hàng: {ex.Message}", null);
            }
        }

        private Dictionary<int, int> GetSessionCart()
        {
            var json = Session.GetString(SessionCartKey);
            if (string.IsNullOrEmpty(json)) return new Dictionary<int, int>();
            try
            {
                return JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
            }
            catch
            {
                return new Dictionary<int, int>();
            }
        }

        private void SaveSessionCart(Dictionary<int, int> cart)
        {
            Session.SetString(SessionCartKey, JsonSerializer.Serialize(cart));
        }
    }
}

