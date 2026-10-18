using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniMarket.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Required, StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Tiện ích tương thích
        [NotMapped]
        public bool IsMain { get => IsPrimary; set => IsPrimary = value; }

        [NotMapped]
        public int SortOrder { get => DisplayOrder; set => DisplayOrder = value; }
    }
}
