using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniMarket.Models
{
    public enum OrderStatus
    {
        Pending = 0,     // Chờ xác nhận
        Confirmed = 1,   // Đã xác nhận
        Shipping = 2,    // Đang giao hàng
        Completed = 3,   // Hoàn thành
        Cancelled = 4    // Đã hủy
    }

    public class Order
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string OrderCode { get; set; } = string.Empty;

        public string BuyerId { get; set; } = string.Empty;
        public virtual ApplicationUser? Buyer { get; set; }

        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [Required, StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? District { get; set; }

        [StringLength(100)]
        public string? Ward { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }

        public int ShippingMethod { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public int PaymentMethod { get; set; } = 0; // 0 = COD

        public int PaymentStatus { get; set; } = 0;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        [StringLength(450)]
        public string? CancelledBy { get; set; }

        public DateTimeOffset? CancelledAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        // Tiện ích tương thích
        [NotMapped]
        public string OrderNumber { get => OrderCode; set => OrderCode = value; }

        [NotMapped]
        public string RecipientName { get => FullName; set => FullName = value; }

        [NotMapped]
        public string DeliveryAddress { get => ShippingAddress; set => ShippingAddress = value; }

        [NotMapped]
        public string? Notes { get => Note; set => Note = value; }
    }

    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Required, StringLength(200)]
        public string ProductTitle { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; } = 1;

        public string? SellerId { get; set; }

        [StringLength(500)]
        public string? ProductImageUrl { get; set; }

        // Tiện ích tương thích
        [NotMapped]
        public string ProductNameSnapshot { get => ProductTitle; set => ProductTitle = value; }

        [NotMapped]
        public decimal UnitPrice { get => ProductPrice; set => ProductPrice = value; }

        [NotMapped]
        public decimal Subtotal => ProductPrice * Quantity;

        [NotMapped]
        public string? ProductImageSnapshot { get => ProductImageUrl; set => ProductImageUrl = value; }
    }
}
