using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;
using UniMarket.ViewModels;

namespace UniMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hỗ trợ cả IndexAdmin và Index
        [HttpGet]
        public async Task<IActionResult> IndexAdmin()
        {
            return await GetDashboardDataAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await GetDashboardDataAsync();
        }

        private async Task<IActionResult> GetDashboardDataAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var pendingProducts = await _context.Products.CountAsync(p => p.Status == ProductStatus.Pending);
            var approvedProducts = await _context.Products.CountAsync(p => p.Status == ProductStatus.Approved);
            var totalOrders = await _context.Orders.CountAsync();
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            var totalOrderValue = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);

            var recentProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Owner)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentOrders = await _context.Orders
                .Include(o => o.Buyer)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                PendingProducts = pendingProducts,
                ApprovedProducts = approvedProducts,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                TotalOrderValue = totalOrderValue,
                RecentProducts = recentProducts,
                RecentOrders = recentOrders,
                RecentUsers = recentUsers
            };

            return View("IndexAdmin", viewModel);
        }
    }
}
