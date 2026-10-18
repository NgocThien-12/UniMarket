using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniMarket.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? IconClass { get; set; }

        [StringLength(50)]
        public string? ColorClass { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        // Tiện ích tương thích
        [NotMapped]
        public string? Icon => IconClass ?? "📦";

        private string? _slug;
        [NotMapped]
        public string? Slug 
        { 
            get => _slug ?? Name.ToLower().Replace(" ", "-").Replace("&", ""); 
            set => _slug = value; 
        }

        [NotMapped]
        public int SortOrder { get => DisplayOrder; set => DisplayOrder = value; }
    }
}
