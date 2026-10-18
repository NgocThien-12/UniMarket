using UniMarket.Models;

namespace UniMarket.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int PendingProducts { get; set; }
        public int ApprovedProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalOrderValue { get; set; }

        public List<Product> RecentProducts { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
        public List<ApplicationUser> RecentUsers { get; set; } = new();
    }

    public class AdminProductListViewModel
    {
        public List<Product> Products { get; set; } = new();
        public string? Keyword { get; set; }
        public ProductStatus? Status { get; set; }
        public int? CategoryId { get; set; }
        public List<Category> Categories { get; set; } = new();
        public int TotalItems { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }

    public class AdminUserListViewModel
    {
        public List<ApplicationUser> Users { get; set; } = new();
        public Dictionary<string, string> UserRoles { get; set; } = new();
        public string? Keyword { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }

    public class AdminOrderListViewModel
    {
        public List<Order> Orders { get; set; } = new();
        public string? Keyword { get; set; }
        public OrderStatus? Status { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}

