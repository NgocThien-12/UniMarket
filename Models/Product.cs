using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniMarket.Models
{
    public enum ProductStatus
    {
        Pending = 0,    // Chờ duyệt
        Approved = 1,   // Đã duyệt
        Rejected = 2,   // Bị từ chối
        Inactive = 3,   // Ẩn / Tạm dừng
        Sold = 4        // Đã bán
    }

    public enum ProductCondition
    {
        New = 0,            // Mới
        LikeNew = 1,        // Còn rất tốt
        Good = 2,           // Còn tốt
        Used = 3            // Đã qua sử dụng
    }

    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OriginalPrice { get; set; }

        public ProductCondition Condition { get; set; } = ProductCondition.Used;

        [StringLength(200)]
        public string? Location { get; set; }

        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public string OwnerId { get; set; } = string.Empty;
        public virtual ApplicationUser? Owner { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Approved;

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Thuộc tính tiện ích tương thích
        [NotMapped]
        public string Name { get => Title; set => Title = value; }

        [NotMapped]
        public string SellerId { get => OwnerId; set => OwnerId = value; }

        [NotMapped]
        public ApplicationUser? Seller { get => Owner; set => Owner = value; }

        [NotMapped]
        public bool IsAvailable => Status == ProductStatus.Approved;

        [NotMapped]
        public int AvailableQuantity { get => Status == ProductStatus.Sold ? 0 : 1; set { } }
    }
}
