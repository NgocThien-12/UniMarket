using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniMarket.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [StringLength(100)]
        public string? SessionId { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        [NotMapped]
        public decimal Total => Items.Sum(i => (i.Product?.Price ?? 0) * i.Quantity);

        [NotMapped]
        public int TotalQuantity => Items.Sum(i => i.Quantity);
    }

    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }
        public virtual Cart? Cart { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.UtcNow;

        [NotMapped]
        public decimal UnitPrice => Product?.Price ?? 0;

        [NotMapped]
        public decimal Subtotal => (Product?.Price ?? 0) * Quantity;
    }
}
