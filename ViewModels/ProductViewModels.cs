using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using UniMarket.Models;

namespace UniMarket.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Category> Categories { get; set; } = new();
        public List<Product> NewestProducts { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
        public List<Product> StudentBudgetProducts { get; set; } = new();
    }

    public class ProductFilterViewModel
    {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public string? CategorySlug { get; set; }
        public string? PriceRange { get; set; } // under100, 100to500, 500to1000, over1000
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public ProductCondition? Condition { get; set; }
        public string? Location { get; set; }
        public string SortBy { get; set; } = "newest"; // newest, price-asc, price-desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;

        // Kết quả truy vấn
        public List<Product> Products { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        // Danh sách hỗ trợ lọc
        public List<Category> Categories { get; set; } = new();
    }

    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề tin đăng.")]
        [StringLength(150, ErrorMessage = "Tiêu đề không vượt quá 150 ký tự.")]
        [Display(Name = "Tiêu đề sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán.")]
        [Range(0, 1000000000, ErrorMessage = "Giá bán không hợp lệ.")]
        [Display(Name = "Giá bán (VNĐ)")]
        public decimal Price { get; set; }

        [Display(Name = "Tình trạng sản phẩm")]
        public ProductCondition Condition { get; set; } = ProductCondition.Good;

        [Required(ErrorMessage = "Vui lòng nhập khu vực/địa điểm giao dịch.")]
        [StringLength(200, ErrorMessage = "Địa điểm không vượt quá 200 ký tự.")]
        [Display(Name = "Địa điểm")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Số lượng có sẵn")]
        [Range(1, 100, ErrorMessage = "Số lượng từ 1 đến 100.")]
        public int AvailableQuantity { get; set; } = 1;

        [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết sản phẩm.")]
        [StringLength(3000, ErrorMessage = "Mô tả không vượt quá 3000 ký tự.")]
        [Display(Name = "Mô tả chi tiết")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Hình ảnh sản phẩm (tối đa 5 ảnh)")]
        public List<IFormFile>? Images { get; set; }

        public List<Category>? Categories { get; set; }
    }

    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = null!;
        public List<Product> RelatedProducts { get; set; } = new();
        public bool IsFavorite { get; set; }
        public bool IsOwner { get; set; }
    }
}

